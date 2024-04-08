# Setup

Required Components.
- dotnet sdk
- make - Can install via chocolatey
- reportgenerator `dotnet tool install --global dotnet-reportgenerator-globaltool`

After using this template repository to create a new repository you will need to setup your dotnet solution/project.

To create a dotnet project/sln use the `dotnet new` command.

The `Makefile` assumes there will be a solution with multiple projects. At least one for a project and one for testing the project.

`dotnet new sln -n Momentum.SOLUTION_NAME`

If you are not sure which type of project to create use `dotnet new --list` to see the list of project templates currently available on your machine. More project templates can be downloaded. 

If you are creating a general library that will become a nuget package use:
`dotnet new classlib -n Momentum.{LIBRARY_NAME} -o src/Momentum.{LIBRARY_NAME}`

Add this project to the solution.
`dotnet sln add src/Momentum.{LIBRARY_NAME}`

Create a test project for testing this project.
`dotnet new xunit -n Momentum.{LIBRARY_NAME}.Tests -o tests tests/Momentum.{LIBRARY_NAME}.Tests`

Add the test project to the solution.
`dotnet sln add tests/Momentum.{LIBRARY_NAME}.Tests`

Add the project as a dependency to the test project.
`cd tests/Momentum.{LIBRARY_NAME}.Tests`
`dotnet add reference ../../src/Momentum.{LIBRARY_NAME}`

Other useful packages for your test project are:
- Moq - Useful for mocking interfaces `dotnet add package Moq`
- Coverlet - Building coverage reports - `dotnet add package Coverlet.MsBuild`

# Makefile
Found at ./Makefile

The make file assumes a Windows Environment. 

There are commands to alias dotnet commands like
`make clean` (dotnet clean)
`make restore` (dotnet restore)
`make build` (dotnet build)
`make test` (dotnet test)

There is a command to create a unit test code coverage report as well, expect `reportgenerator` to be installed.
`make test-coverage`

A command to create nuget packages from your solution projects is available here.
`make pack`
This generates a version 1.0.0 nuget package for your projects in the `.\nuget` directory.
To provide a different version use: `make pack VERSION={version_value}` like:
`make pack VERSION=1.0.1-alpha`.

# Github Nuget Packages Setup
Use the `make setup-github-nuget GH_USER={username} GH_TOKEN={PAT}`
The personal access token being used must have at least read access for packages.