# Create Script Folders and Assemblies with Tests

This Unity Editor Extensions create script folders (Editor, Runtime, and each Tests) containing assembly definition file (.asmdef).

When opening the context menu and select **Create | C# Script Folders and Assemblies with Tests**,
The root folder (e.g., named "MyFeature") and below will be created as follows.

```
MyFeature
├── Scripts
│   ├── Editor
│   │   └── MyFeature.Editor.asmdef
│   └── Runtime
│       └── MyFeature.asmdef
└── Tests
    ├── Editor
    │   └── MyFeature.Editor.Tests.asmdef
    └── Runtime
        └── MyFeature.Tests.asmdef
```


## More information

See [GitHub repository](https://github.com/nowsprinting/create-script-folders-with-tests) for more information.
