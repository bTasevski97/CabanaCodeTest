# AI-Assisted Workflow Documentation

This document outlines the workflow used to build the Resort Map application using modern AI-assisted development tools.

## Tooling Used
- **Antigravity IDE:** An agentic IDE used for end-to-end development.
- **AI Models:** Leveraged both **Claude** and **Gemini** models through agentic capabilities just because I had access to them. For some tasks, I would prefer to use **GPT** models.


## Workflow Summary

### 1. Rapid Prototyping & Iteration
I adopted a "rapid-first" strategy, focusing on achieving a fully working scenario as quickly as possible. This meant prioritizing core functionality (map rendering, basic booking) over initial code polish. Once the baseline was working, I iterated rapidly to refine the solution and address edge cases.

### 2. Harnessing AI "Slop"
The initial agent-generated code often contained "slop" - redundant or slightly off-patterns common in one-shot generation. I utilized the agents to specifically target and clean this up, "harnessing" it through repeated iteration until the code structure and quality matched my requirements.

### 3. Agentic Exploration (No "Core Prompts")
There were no complex, pre-defined "core prompts." Instead, I used **natural language** to describe what I wanted to achieve and allowed the agents to explore the codebase and requirements themselves. This autonomy enabled the agents to identify dependencies, project structure, and potential issues without **context engineering**.

### 4. README as PBI
I used the provided `README.md` (the original task description) as the primary **Product Backlog Item (PBI)** for both myself and the agents. This ensured alignment with the business requirements while allowing the AI to treat the instructions as a source of truth for implementation logic.

### 5. AI-Generated Test Suites
At the end of the development cycle, I tasked the agents with generating comprehensive test suites for both the frontend (React/Vitest) and the backend (.NET/xUnit). The AI was able to leverage the existing context to create meaningful tests that covered the core business logic and user flows.

## Metrics
- **Automated Implementation:** >95% of the codebase was written by AI agents under natural language direction.
