using Microsoft.Agents.AI;                  // core agent types
using Microsoft.Extensions.AI;
                                            // AIAgent, etc.
                                            // using Microsoft.Agents.AI.OpenAI;        // if switching to OpenAI
using OllamaSharp;        // Ollama support (preview/experimental)

HangmanGamePlugin game = new();
// ... your chatClient setup
var chatClient = new OllamaApiClient("http://localhost:11434", "qwen2.5:32b-instruct-q5_K_M");

// Create two guesser agents (both can call the shared plugin functions as tools)
var agentA = chatClient.CreateAIAgent(
    name: "AgentA",
    instructions: FormatGuesserInstructions("AgentA", "careful"),
    tools: CreateToolsFromPlugin(game)   // your GuessLetter + GetGameState as AIFunctions
);

var agentB = chatClient.CreateAIAgent(
    name: "AgentB",
    instructions: FormatGuesserInstructions("AgentB", "chaotic"),
    tools: CreateToolsFromPlugin(game)
);

// Thread for shared conversation history (both agents see everything)
var thread = agentA.GetNewThread();

// Kick off the game
var initialMessages = new[]
{
    new ChatMessage(ChatRole.System, game.GetGameState())
};

game.StartNewGame();
Console.WriteLine("\n=== GAME STARTED ===");
Console.WriteLine($"[System] -> {game.GetGameState()}");

var agents = new[] { agentA, agentB };
int currentIndex = 0;
int maxTurns = 60;


for (int turn = 1; turn <= maxTurns; turn++)
{
    var currentAgent = agents[currentIndex];
    currentIndex = (currentIndex + 1) % agents.Length;

    Console.WriteLine($"\n[Turn {turn} - {currentAgent.Name}]");

    // This is the key call — agent thinks, calls tools if needed, responds
    var response = await currentAgent.RunAsync(initialMessages, thread, new AgentRunOptions
    {
        AdditionalProperties = new AdditionalPropertiesDictionary
        {
            ["temperature"] = 0.0f,
            ["Temperature"] = 0.0f,
            ["max_tokens"] = 50,
            ["max_new_tokens"] = 50,
            ["MaxTokens"] = 50,
            ["maxTokens"] = 50
        }
    });

    if (response.Text is { Length: > 0 } content)
    {
        Console.WriteLine($"[{currentAgent.Name}] -> {content}");

        if (content.Contains("Game Over", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("win", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("\n=== GAME FINISHED ===");
            break;
        }
    }
    else
    {
        Console.WriteLine("(no meaningful response)");
    }

    // Optional: small delay for readability
    await Task.Delay(300);
}

Console.WriteLine("Max turns reached or game ended.");


static string FormatGuesserInstructions(string name, string type) => $@"You are a {type} hangman game player.
Your name is {name}, do NOT change this.
Every turn:
0. Read the most recent message starting with 'Word:'. Extract Word, Guessed, and Wrongs.
1. Call GuessLetter() ONLY ONCE per turn with a random letter which is not after 'Guessed:' and your name.
2. Pass the result from GuessLetter() EXACTLY ('Word: _ _ _' | 'Guessed: a' | 'Wrongs: 0/6') to the chat message
3. End condition:
 - If Word: has '_' → say the result form GuessLetter()
 - If Word: has no '_' → say ""Game Over - You win!""
 - If Wrongs: 6/6 → say ""Game Over - You lose!"" 

Guess AT MOST ONE letter.
DO NOT add more letters to Guessed, pass the result from GuessLetter() EXACTLY
Please respond clearly and concisely without <tool_call>";

static IList<AITool> CreateToolsFromPlugin(HangmanGamePlugin game)
{
    return new[]
    {
        AIFunctionFactory.Create(
            (string letter, string agentName) => game.GuessLetter(letter, agentName),
            "GuessLetter",
            "First parameter: ONE single lowercase letter. Second parameter: your name."
        )
    };
}

public class HangmanGamePlugin
{
    private string _secretWord = "";
    private char[] _Word = [];
    private readonly HashSet<char> _guessed = [];
    private int _wrongs = 0;
    private const int MaxWrongs = 6;

    // [KernelFunction, Description("Do not call this, the game is already started")]
    public string StartNewGame()
    {
        var words = new[] { "test", "hang", "four", "exam", "grok", "code", "game" };
        _secretWord = words[Random.Shared.Next(words.Length)].ToLower();
        _Word = Enumerable.Repeat('_', _secretWord.Length).ToArray();
        _guessed.Clear();
        _wrongs = 0;
        var result = $"New game started! Word has {_secretWord.Length} letters: {string.Join(' ', _Word)}";
        Console.WriteLine($"[DEBUG] StartNewGame -> {result}");
        return result;
    }

    //[KernelFunction, Description("ONLY use this to guess ONE single lowercase letter. Parameter must be exactly one letter a-z. Do NOT use ResetGuess or any other invented name.")]
    public string GuessLetter(string letter, string agentName)
    {
        Console.WriteLine($"[{agentName}] GuessLetter called with: {letter}");
        if (letter.Length != 1 || !char.IsLetter(letter[0])) return "Invalid: guess one lowercase letter.";
        char c = char.ToLower(letter[0]);
        if (_guessed.Contains(c)) return $"Already guessed '{c}'.";
        _guessed.Add(c);
        if (_secretWord.Contains(c))
        {
            for (int i = 0; i < _secretWord.Length; i++)
                if (_secretWord[i] == c) _Word[i] = c;
            if (!_Word.Contains('_'))
            {
                var winMsg = $"Correct! The word was '{_secretWord}'. You win!";
                //Console.WriteLine($"[DEBUG] {winMsg}");
                return winMsg;
            }
        }
        else
        {
            _wrongs++;
            if (_wrongs >= MaxWrongs)
            {
                var loseMsg = $"Wrong! The word was '{_secretWord}'. Hangman wins!";
                //Console.WriteLine($"[DEBUG] {loseMsg}");
                return loseMsg;
            }
            var result0 = $"Word: {string.Join(' ', _Word)} | Guessed: {string.Join(",", _guessed)} | Wrongs: {_wrongs}/{MaxWrongs}";
            //Console.WriteLine($"[DEBUG] GuessLetter result -> {result0}");
            return result0;
        }
        var result = $"Word: {string.Join(' ', _Word)} | Guessed: {string.Join(",", _guessed)} | Wrongs: {_wrongs}/{MaxWrongs}";
        //Console.WriteLine($"[DEBUG] GuessLetter result -> {result}");
        return result;
    }

    //[KernelFunction, Description("ONLY use this to check the current game board, guessed letters, and wrongs. Do NOT invent other state functions.")]
    public string GetGameState()
    {
        var state = $"Word: {string.Join(' ', _Word)} | Guessed: {string.Join(",", _guessed)} | Wrongs: {_wrongs}/{MaxWrongs}";
        //Console.WriteLine($"[DEBUG] GetGameState -> {state}");
        return state;
    }
}