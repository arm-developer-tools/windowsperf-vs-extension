# WperfVersion Class Documentation

[[_TOC_]]

## Summary

The `WperfVersion` class is a part of the `WindowsPerfGUI.SDK.WperfOutputs` namespace. It provides a representation of the version information for components in the Windows Performance (Wperf) system. It is capable of parsing this information from a JSON string.
The type is taken from [this schema](https://gitlab.com/Linaro/WindowsPerf/windowsperf/-/blob/sampling/wperf-scripts/tests/schemas/wperf.version.schema).

## Properties

### WperfVersion Class

- `List<Version> Version`: This property holds a list of `Version` objects, each representing the version information of a component.

### Version Class

- `string Component`: The name of the component.
- `string ComponentVersion`: The version of the component.

## Methods

### WperfVersion Class Methods

- `static WperfVersion FromJson(string json)`: This static method accepts a JSON string as a parameter and returns a `WperfVersion` object. The JSON string should represent the version information for one or more components in the Wperf system.

## Usage Example

```csharp
string json = @"{
  ""Version"": [
    {
      ""Component"": ""wperf"",
      ""Version"": ""2.4.0""
    },
    {
      ""Component"": ""wperf-driver"",
      ""Version"": ""2.4.0""
    }
  ]
}";

// Parse the JSON into a WperfVersion object
WperfVersion wperfVersion = WperfVersion.FromJson(json);

// Print each component's name and version
foreach (Version version in wperfVersion.Version)
{
    Console.WriteLine($"Component: {version.Component}, Version: {version.ComponentVersion}");
}
```

In this example, a JSON string containing version information for two components is parsed into a `WperfVersion` object. The name and version of each component are then written to the console.
