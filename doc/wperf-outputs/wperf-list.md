# WperfList Class

[[_TOC_]]

The `WperfList` class is a part of the `WindowsPerfGUI.SDK.WperfOutputs` namespace. It represents a collection of predefined events and metrics for `wperf`.

## Summary

The `WperfList` class contains two properties: `PredefinedEvents` and `PredefinedMetrics`. These properties hold lists of predefined events and metrics, respectively.
The type is taken from [this schema](https://gitlab.com/Linaro/WindowsPerf/windowsperf/-/blob/sampling/wperf-scripts/tests/schemas/wperf.list.schema).

## Methods / Properties

### PredefinedEvents

- Type: `List<PredefinedEvent>`
- Description: Gets or sets the list of predefined events.
- Property Example:

  ```csharp
  List<PredefinedEvent> PredefinedEvents { get; set; }
  ```

### PredefinedMetrics

- Type: `List<PredefinedMetric>`
- Description: Gets or sets the list of predefined metrics.
- Property Example:

  ```csharp
  List<PredefinedMetric> PredefinedMetrics { get; set; }
  ```

## Example Usage

The following example demonstrates how to use the `WperfList` class:

```csharp
using System;
using System.Collections.Generic;
using WindowsPerfGUI.SDK.WperfOutputs;

class Program
{
    static void Main()
    {
        string json = "{\"Predefined_Events\":[{\"Alias_Name\":\"Event1\",\"Event_Type\":\"Type1\",\"Raw_Index\":\"Index1\"}],\"Predefined_Metrics\":[{\"Events\":\"Event1\",\"Metric\":\"Metric1\"}]}";

        WperfList wperfList = WperfList.FromJson(json);

        Console.WriteLine("Predefined Events:");
        foreach (PredefinedEvent predefinedEvent in wperfList.PredefinedEvents)
        {
            Console.WriteLine($"Alias Name: {predefinedEvent.AliasName}");
            Console.WriteLine($"Event Type: {predefinedEvent.EventType}");
            Console.WriteLine($"Raw Index: {predefinedEvent.RawIndex}");
            Console.WriteLine();
        }

        Console.WriteLine("Predefined Metrics:");
        foreach (PredefinedMetric predefinedMetric in wperfList.PredefinedMetrics)
        {
            Console.WriteLine($"Events: {predefinedMetric.Events}");
            Console.WriteLine($"Metric: {predefinedMetric.Metric}");
            Console.WriteLine();
        }
    }
}
```

This example creates a `WperfList` object from a JSON string. It then accesses the `PredefinedEvents` and `PredefinedMetrics` properties to display their values in the console.
