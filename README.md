# Two Player Pong

A classic two-player Pong game built with C# and Windows Forms (.NET Framework 4.7.2).

## Gameplay

Two players compete on the same keyboard to score points by getting the ball past the opponent's paddle. The match runs for 5 minutes, and the player with the most points when the timer hits zero wins.

The ball speeds up every 10 seconds to keep things interesting.

## Controls

| Player | Move Up | Move Down |
|--------|---------|-----------|
| Player 1 (left) | `W` | `S` |
| Player 2 (right) | `↑` | `↓` |

## Features

- 5-minute timed matches
- Ball speed increases over time
- Per-session score history shown at game end
- High scores saved to `highscores.txt` and persist between sessions

## Requirements

- Windows
- .NET Framework 4.7.2
- Visual Studio (to build)

The following image assets must be in the same directory as the executable:
- `background.png`
- `block.png` (Player 1 paddle)
- `block1.png` (Player 2 paddle)
- `ball.png`

## Running

Open `TwoPlayerPong.sln` in Visual Studio and hit Run, or build and run the executable from `bin/Debug/`.
