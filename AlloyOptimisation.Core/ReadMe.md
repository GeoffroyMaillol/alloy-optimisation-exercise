# Alloy optimisation exercise

This project implements alloy optimisation.
It was mostly designed test first, letting the requirements dictate the program's behaviour.

## Design

The module articulates around the Alloy class that represents an alloy, containing a list of components with their atomic percentage.
The class validates the alloy's composition against the alloy configuration.

This configuration is provided through the AlloyComponentConfigProvider abstract class, which is implemented to load such configuration from a JSON file into a record object.
The class can be implemented to represent various alloys, each pointing to a different JSON file.

The JSON file should define each allowed component for the alloy, with:
* the element chemical symbol (e.g. Ni for nickel)
* the creep coefficient
* the cost in £/kg
* the minimum atomic percentage allowed
* the maximum atomic percentage allowed
* the step by which to increment / decrement the atomic percentage

Please note that the creep coefficient of the alloy's base element should be set to null, it is how the code determines that it is the base element. 
All other properties should be set.

The last piece is the AlloyOptimiser, which uses a recursive method to calculate each possible alloy composition within the range defined by the configuration.
It is not optimal (wasteful even), but it was deemed sufficient for the purpose of this exercise.

## Running the tests

Unit tests can be run from the root of the AlloyOptimisation.Tests project with:
```
dotnet test
```
