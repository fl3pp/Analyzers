.PHONY: all build clean clean

clean:
	dotnet clean

build:
	dotnet build --configuration Release

pack:
	dotnet pack --configuration Release --no-build

all: clean build pack