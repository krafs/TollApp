The following changes 

### Add automatic tests
This ensures the code adheres to the requirements and good coding practices, and helps prevent regressions during refactors and other changes.

### Fix failing tests
Many of the requirements weren't met. For example, public holidays were just hardcoded American holidays for 2013, it could not handle the sliding window of free passages, etc. These were fixed.

### Make TollCalculator configurable at runtime
Many values in the business logic are hardcoded. This means that any changes to e.g. fees, schedule and exempt vehicle types requires re-compilation of the app and re-deployment. Although that can be fine for some apps, an app like this is most likely not.

Having all variable values be injectable in the constructor allows them to be retrieved from, and updated via, a configuration file, database or external service.
Conventional approaches involve injecting some kind of async service interface for config retrieval, but I prefer keeping business logic and the interaction of external services separate. It makes it easier to reason about, and easier to test.

TollCalculatorConfig is not the most elegant way to handle configuration. The rules engine in particular could use some more work, but it represents the idea.

### Rename public API
The functions for calculating the toll for a single passage, and for calculating all the passes during a whole day, have the same name. They are just different overloads. This is confusing, and makes it difficult to tell them apart without looking closer. 

### Improve error handling
Ensure invalid input is handled appropriately, like throwing ArgumentNullException on null Vehicles. Swallowing bad data and returning e.g. 0 if Vehicle is null makes the contract confusing.

Also, changing the method signature from Method(Vehicle vehicle, DateTime[] dates) to Method(Vehicle vehicle, DateOnly date, IEnumerable<TimeOnly> times) moves a potential runtime error to compile time. The function is only for calculating the toll for all passes in a single day, so if passing in DateTimes for multiple separate days would force us to throw a runtime exception. Passing in a DateOnly removes that issue altogether.

### Simplify abstraction
The original Vehicle abstraction could be useful if behaviour and data for different kinds of vehicles was more elaborate, but it's just a holder of an enum. I turned the Vehicle interface into a concrete class instead, and just indicate the vehicle type with the enum directly. Keeping the Vehicle class still allows us to add more to it in the future if needed. 
