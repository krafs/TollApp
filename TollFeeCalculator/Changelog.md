### Changes made, in order of priority:

- Add automatic tests to ensure code adheres to requirements and to prevent regressions.
- Rewrite code to actually pass requirement tests.
- Make values in TollCalculator configurable at runtime to allow for changes without re-compile/re-deploy, e.g. change in fee. TollCalculatorConfig is perhaps a bit clumsy, especially with the rules engine, but it illustrates the point.
- Rename functions for clarity
- Improve error prevention- and handling.
- Remove unnecessary Vehicle abstraction. Still possible to re-add in future if required.