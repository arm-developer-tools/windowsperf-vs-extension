# WperfTest Class

[[_TOC_]]

The `WperfTest` class is a part of the `WindowsPerfGUI.SDK.WperfOutputs` namespace. It represents a collection of test results that represents additional data about the host for `wperf`.

## Summary

The `WperfTest` class contains a property named `TestResults`, which is a list of `TestResult` objects. Each `TestResult` object represents a single test result and contains information about the result and the test name.
[this Schema](https://gitlab.com/Linaro/WindowsPerf/windowsperf/-/blob/sampling/wperf-scripts/tests/schemas/wperf.test.schema)

## Methods / Properties

### TestResults

- Type: `List<TestResult>`
- Description: Gets or sets the list of test results.
- Property Example:

  ```csharp
  List<TestResult> TestResults { get; set; }
  ```

## Example Usage

The following example demonstrates how to use the `WperfTest` class:

```csharp
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WindowsPerfGUI.SDK.WperfOutputs;

class Program
{
    static void Main()
    {
        string json = "{\"Test_Results\":[{\"Result\":\"Pass\",\"Test_Name\":\"Test1\"},{\"Result\":\"Fail\",\"Test_Name\":\"Test2\"}]}";

        WperfTest wperfTest = WperfTest.FromJson(json);

        Console.WriteLine("Test Results:");
        foreach (TestResult testResult in wperfTest.TestResults)
        {
            Console.WriteLine($"Test Name: {testResult.TestName}");
            Console.WriteLine($"Result: {testResult.Result}");
            Console.WriteLine();
        }
    }
}
```

This example creates a `WperfTest` object from a JSON string. It then accesses the `TestResults` property to iterate over the test results and display their test names and results in the console.
