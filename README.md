This starter is a rewrite of my Semantic Kernel practice project.  
For more details on my journey to make my first AI project and tips see:  
[My LinkedIn article: "Multi agent Hangman game with Semantic Kernel Practice project"](https://www.linkedin.com/pulse/multi-agent-hangman-game-semantic-kernel-practice-patrick-koorevaar-1mvef/?trackingId=CU8au4NVTjmTsNrn8MXoVw%3D%3D)

### Local Inference Setup (Intel GPU Optimized via IPEX-LLM — Recommended for Windows!)
This example uses a portable Ollama build with **IPEX-LLM** integration for fast, stable local inference on Intel GPUs (Arc / Xe Graphics).

**Why this build?**
- Standard Ollama can lag (> 60s responses on 32B models) or reboot frequently without full optimization.
- IPEX-LLM version: Smooth, high-token-rate runs on Intel hardware — no NVIDIA needed!

**Tested on**: Windows with Intel Arc iGPU + latest drivers.

**Quick Setup Steps** (Portable — No Full Install Needed):
1. **Update Intel Drivers** (essential for stability!):  
   Download latest from [intel.com/support](https://www.intel.com/content/www/us/en/support/products/80939/graphics.html) (search your GPU model).
2. **Download IPEX-LLM Ollama Portable Zip**:  
   Get the latest Windows version from Intel's guide:  
   [GitHub Quickstart - Ollama Portable Zip](https://github.com/intel/ipex-llm/blob/main/docs/mddocs/Quickstart/ollama_portable_zip_quickstart.md)  
   (Your ollama-ipex-llm-2.2.0-win is solid; upgrade if needed for newer features.)

   [Or get it here](https://github.com/ipex-llm/ipex-llm/releases/tag/v2.2.0)
4. **Unzip & Start**:  
   - Extract the zip to a folder.  
   - Run `start-ollama.bat` (or equivalent in latest zip).  
5. **Pull a Model**:  
   In a command prompt:  
   `ollama pull qwen2.5:32b-instruct-q5_K_M`  
   (Quantized GGUF models recommended for max speed.)
6. **Run the Server**:  
   Ollama serves automatically on startup — accessible at `http://localhost:11434`.

NVIDIA/Standard Ollama users: Use official Ollama download + CUDA for equivalent performance.

Feedback welcome if issues on your hardware!


## Progress & What's Next
This repo is a living learning project: I'm gradually evolving a multi-agent Hangman game to explore modern patterns in **Microsoft Agent Framework** (the successor direction after AutoGen/Semantic Kernel agent chat experiments).

### Current Milestone (as of January 2026)
- Basic multi-agent setup with Ollama IPEX-LLM for local Intel GPU inference
- Game logic via plugin/tools
- Manual chat loop

### Planned Next Steps (approximate order, subject to change)
1. Replace manual chat loop by GroupChat
2. Add Coordinator agent who does game start and turn selection
3. Dynamic guesser count (2–4 players randomized per game)
4. Let Coordinator start new game with random players after game ends


I'm adding one meaningful improvement roughly every 1–2 weeks when time allows.  
Feel free to ⭐ watch the repo if you're interested in following along, or open an issue/discussion with suggestions/questions — feedback on Intel GPU setups or prompt ideas is especially welcome!

Previous versions will get **tags** (e.g. `v0.1-initial`, `v0.2-groupchat`, etc.) so you can checkout exactly how it looked at each stage.
