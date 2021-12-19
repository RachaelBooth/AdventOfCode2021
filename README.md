# README

Advent of code is an excellent set of programming puzzles released each year, which can be found at [adventofcode.com](https://adventofcode.com).

This contains my 2021 solutions and some helpers that I'm slowly building out, which I should at some point put into a separate package but I haven't done that yet - maybe I'll get round to it some day, but I probably won't because it's convenient to keep editing it fairly lightly.

This uses the [AoCHelper](https://github.com/eduherminio/AoCHelper) package for running, while my own helpers (in AoCBase) add some input parsing and other utilities.

Advent of code is purely for fun, so (outside of helpers I might use later, and even then somewhat because I'm very much going for a specific use case) I'm only really writing to a "use this once" standard. Don't judge me!

### Usage

This is really to remind me, to be honest.

For a new day;
1. Add a `SolverXX` class which inherits `BaseSolver`, probably in its own folder so you can split out into several files as needed
2. Copy and paste the input to `Inputs\XX.txt`, and ensure the file is set to be copied to the output directory
3. Solve the problem!

Running the solution will run the most recent day. Edit `Program.cs` to solve all or solve a specific day.