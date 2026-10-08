# TTC Rubik's Cube Challenge

A C# console program that simulates a 3x3 Rubik's Cube and supports 90-degree
turns of every face. It starts solved, with Green at the front, Red on the right,
and White on top.

## Prerequisites

- The free [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) and Runtime,

## Build and run

Check out the repo or extract the source ZIP if necessary. 

Load `TTC_RubikChallenge.slnx` with Visual Studio then build and run.

Or

Open PowerShell in the directory containing
`TTC_RubikChallenge.slnx`, then run these commands in order:

```powershell
dotnet restore
dotnet build -c Release --no-restore
dotnet run --project TTC_RubikChallenge/TTC_RubikChallenge.csproj -c Release --no-build
```

The program displays the cube and instructions. Enter one move per line:

- Faces: `front`, `right`, `top`, `back`, `left`, `bottom`.
  `up` and `down` are also accepted for Top and Bottom.
- Directions: `>` is clockwise; `<` is anticlockwise, viewed directly at the
  selected face from outside the cube. For example, `front >` or `right <`.
- `reset` restores the solved cube.
- Ctrl+Z followed by Enter ends input and closes the program on Windows.

## Required rotation sequence

From a fresh start, enter the following moves in order. If the cube has already
been changed, enter `reset` first:

```text
front >
right <
top >
back <
left >
bottom <
```

These correspond to the brief's Front clockwise, Right anticlockwise,
Up clockwise, Back anticlockwise, Left clockwise, and Down anticlockwise turns.
