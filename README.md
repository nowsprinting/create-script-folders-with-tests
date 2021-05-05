# Create Script Folders with Tests

[![openupm](https://img.shields.io/npm/v/com.nowsprinting.create-script-folders-with-tests?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.nowsprinting.create-script-folders-with-tests/)

This Unity Editor Extensions create a script folders (Editor, Runtime, and each Tests) containing assembly definition file (.asmdef).

When opening the context menu and select Create | Script Folders with Tests,
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


## Installation

If you installed [openupm-cli](https://github.com/openupm/openupm-cli), run the command below

```bash
$ openupm add com.nowsprinting.create-script-folders-with-tests
```

Or open Package Manager window (Window | Package Manager) and add package from git URL 
`https://github.com/nowsprinting/create-script-folders-with-tests.git`.


## License

MIT License


## How to contribute

Open an issue or create a pull request.
