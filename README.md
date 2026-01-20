Local Inference Setup (Intel GPU Optimized via IPEX-LLM — Recommended for Windows!)
This example uses a portable Ollama build with IPEX-LLM integration for fast, stable local inference on Intel GPUs (Arc / Xe Graphics).

Why this build?

Standard Ollama can lag (> 60s responses on 32B models) or reboot frequently without full optimization.
IPEX-LLM version: Smooth, high-token-rate runs on Intel hardware — no NVIDIA needed!
Tested on: Windows with Intel Arc A770 + latest drivers.

Quick Setup Steps (Portable — No Full Install Needed):

Update Intel Drivers (essential for stability!):
Download latest from intel.com/support (search your GPU model).

Download IPEX-LLM Ollama Portable Zip:
Get the latest Windows version from Intel's guide:
GitHub Quickstart - Ollama Portable Zip
(Your ollama-ipex-llm-2.2.0-win is solid; upgrade if needed for newer features.) Or get it here: ollama-ipex-llm-2.2.0

Unzip & Start:

Extract the zip to a folder.
Run start-ollama.bat (or equivalent in latest zip).
Pull a Model:
In a command prompt:
ollama pull qwen2.5:32b-instruct-q5_K_M
(Quantized GGUF models recommended for max speed. You need a model that supports tools and the 'instruct' ones are better for this task)

Run the Server:
Ollama serves automatically on startup — accessible at http://localhost:11434.

NVIDIA/Standard Ollama users: Use official Ollama download + CUDA for equivalent performance.

Feedback welcome if issues on your hardware!
