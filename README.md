<h1 align="center">
  <img src="public/logo.svg" alt="PhishSim Logo" width="96" />
  <br />
  PhishSim
</h1>

<p align="center">
  An educational phishing-simulation tool — learn how phishing attacks work in a safe, controlled, local environment.
</p>

---

## What is PhishSim?

**PhishSim helps cybersecurity students and trainers understand phishing attacks hands-on:**

- Hosts realistic fake login pages that mimic Google, Instagram, Netflix, Twitter, and more
- Captures and displays submitted credentials in the terminal — locally, never transmitted
- Extensible template system — add new simulation pages without touching the server core
- Cross-platform support on Windows, macOS, and Linux
- Simple terminal-based interface for full server control

**`No real credentials leave your machine. Everything runs locally.`**

> [!WARNING]
> PhishSim is strictly for **educational purposes and security awareness training**.
> Never deploy phishing templates on public networks or use this tool maliciously.
> The author is not responsible for misuse.

---

## How It Works

A complete walkthrough of PhishSim running in a controlled local environment:

**1. Start the Application**

<p align="center">
  <img src="PhishSim/screenshots/screenshot1.png" alt="Start Application" width="700" />
</p>

**2. Choose Server Type**

<p align="center">
  <img src="PhishSim/screenshots/screenshot2.png" alt="Choose Server Type" width="700" />
</p>

**3. Select a Template**

<p align="center">
  <img src="PhishSim/screenshots/screenshot3.png" alt="Select a Template" width="700" />
</p>

**4. Template Opens in Browser**

<p align="center">
  <img src="PhishSim/screenshots/screenshot4.png" alt="Template in Browser" width="700" />
</p>

**5. Enter Demo Credentials**

<p align="center">
  <img src="PhishSim/screenshots/screenshot6.png" alt="Enter Demo Credentials" width="700" />
</p>

**6. Captured Credentials Displayed in Terminal**

<p align="center">
  <img src="PhishSim/screenshots/screenshot5.png" alt="Captured Credentials in Terminal" width="700" />
</p>

---

## Installation

### Requirements

- .NET SDK installed ([verify with `dotnet --version`](https://dotnet.microsoft.com/en-us/download))
- Windows, macOS, or Linux

### Steps

Clone the repository and navigate into the project:
```bash
git clone https://github.com/Nmarino8/PhishSim.git
cd PhishSim/PhishSim
```

Build the project:
```bash
dotnet build
```

---

## Usage

Run the project:
```bash
dotnet run
```

Or from a parent folder:
```bash
dotnet run --project PhishSim/PhishSim.csproj
```

Then follow the prompts:

1. Select the server type — currently **Localhost** only
2. Select a template from the list
3. The local server starts and opens your default browser with the chosen template
4. Interact with the page — enter any username and password
5. Watch the terminal — captured input is displayed immediately

---

## Features

- **Fake Login Pages** — hosts realistic simulation pages for multiple services
- **Credential Capture** — username and password inputs are intercepted and shown in the terminal
- **Extensible Templates** — add new templates by dropping a folder into `Templates/` — no core changes needed
- **Cross-platform** — runs on Windows, macOS, and Linux via .NET
- **Terminal Interface** — clean CLI for server control, template selection, and output

---

## Built-in Templates

| Template | Mimics |
|---|---|
| **Google** | Google login page |
| **Instagram** | Instagram login page |
| **Netflix** | Netflix login page |
| **Twitter** | Twitter login page |
| **DevPortal** | Generic developer portal |

> [!NOTE]
> This list is not exhaustive. Additional templates will be added in future updates. You can also create and add your own — see below.

---

## Adding Custom Templates

1. Create a new folder inside `Templates/` — the folder name becomes the template name in PhishSim.

2. Add the following files inside your new folder:

   | File | Required | Purpose |
   |---|---|---|
   | `index.html` | ✅ | Main HTML page for the template |
   | `script.js` | ✅ | Handles form submission and credential capture |
   | `styles.css` | Optional | CSS styling for the page |
   | `images/` | Optional | Images and asset files |

3. Your `index.html` **must** include a form with these exact IDs:
   - `id="loginForm"` on the `<form>` element
   - `id="username"` on the username input
   - `id="password"` on the password input

4. Add this **exact code** to your `script.js`:
```javascript
const form = document.getElementById('loginForm');

form.addEventListener('submit', (e) => {
    e.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    fetch('/demo', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            username: username,
            password: password
        })
    });

    form.reset();
});
```

5. Restart PhishSim — the new template appears automatically in the selection menu.

> [!TIP]
> The server auto-detects new templates on startup. No changes to the core code are ever needed.

---

## Project Structure
```bash
PhishSim/
├── CLI/                        # Command-line interface
│   ├── CommandRouter.cs
│   ├── ConsoleUI.cs
│   └── Commands/
│       ├── LaunchCommand.cs
│       └── ListCommand.cs
├── Server/                     # Local server logic and template serving
│   ├── LocalServer.cs
│   ├── RequestFilter.cs
│   └── ServerConfig.cs
├── Templates/                  # Simulation website templates
│   ├── DevPortal/
│   ├── Google/
│   ├── Instagram/
│   ├── Netflix/
│   └── Twitter/
├── Utils/                      # Helper utilities
│   └── TerminalAnimation.cs
├── PhishSim.csproj             # .NET project file
└── Program.cs                  # Main entry point
```

---

## Data & Privacy

PhishSim captures no real credentials. All input submitted through simulation pages is intercepted locally and displayed only in your terminal. Nothing is stored, logged to disk, or transmitted outside your machine.

> [!NOTE]
> PhishSim is inspired by **BlackEye** as a foundation for educational phishing simulations.
> Thanks to the cybersecurity community for guidance, insights, and best practices.

> [!WARNING]
> This tool is intended for **learning, training, and security awareness only**.
> Always use PhishSim responsibly. The author is not responsible for any misuse.

---

<h3 align="center">
  PhishSim welcomes contributions — new templates, server improvements, bug fixes, and CLI enhancements.<br />
  Fork the repo, make your changes, and open a Pull Request. Use Issues for bugs, features, and questions.
</h3>

---

<p align="center">
  © 2026 Niko Marinović. All rights reserved.
</p>
