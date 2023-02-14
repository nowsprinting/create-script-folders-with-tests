# Create Script Folders and Assemblies with Tests

[![Meta file check](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/metacheck.yml/badge.svg)](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/metacheck.yml)
[![Test](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/test.yml/badge.svg)](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/test.yml)
[![openupm](https://img.shields.io/npm/v/com.nowsprinting.create-script-folders-with-tests?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.nowsprinting.create-script-folders-with-tests/)

This Unity Editor Extensions create script folders (Editor, Runtime, and each Tests) containing assembly definition file (.asmdef).


### Using under Assets folder:

When opening the context menu and select **Create | C# Script Folders and Assemblies with Tests**,
The root folder (e.g., named "YourFeature") and below will be created as follows.

```
Assets
└── YourFeature
   ├── Scripts
   │   ├── Editor
   │   │   └── YourFeature.Editor.asmdef
   │   └── Runtime
   │       └── YourFeature.asmdef
   └── Tests
       ├── Editor
       │   └── YourFeature.Editor.Tests.asmdef
       └── Runtime
           └── YourFeature.Tests.asmdef
```

And the references of each asmdef are set as follows.

- `YourFeature` has not references
- `YourFeature.Editor` has references to `YourFeature`
- `YourFeature.Tests` has references to `YourFeature`
- `YourFeature.Editor.Tests` has references to `YourFeature` and `YourFeature.Editor`

It also creates DotSettings files including disable [Namespace provider](https://www.jetbrains.com/help/rider/Refactorings__Adjust_Namespaces.html) setting.


### Using under Packages folder:

First, your package folder (e.g., named "your.package.name") must be created in advance.
Because can not open the context menu directly under the Packages folder.

Next, opening the context menu and select **Create | C# Script Folders and Assemblies with Tests**,
The root folder (e.g., named "YourFeature") and below will be created as follows.

```
Packages
└── your.package.name
   └── YourFeature
      ├── Editor
      │   └── YourFeature.Editor.asmdef
      ├── Runtime
      │   └── YourFeature.asmdef
      └── Tests
          ├── Editor
          │   └── YourFeature.Editor.Tests.asmdef
          └── Runtime
              └── YourFeature.Tests.asmdef
```

After creating folders, move the Editor, Runtime and Tests folders directly under the "your.package.name" folder.
And remove the "YourFeature" folder.

Then it will be the same as the official [package layout](https://docs.unity3d.com/Manual/cus-layout.html).


## Installation

If you installed [openupm-cli](https://github.com/openupm/openupm-cli), run the command below

```bash
openupm add com.nowsprinting.create-script-folders-with-tests
```

Or open Package Manager window (Window | Package Manager) and add package from git URL

```
https://github.com/nowsprinting/create-script-folders-with-tests.git
```


## License

MIT License


## How to contribute

Open an issue or create a pull request.

Be grateful if you could label the PR as `enhancement`, `bug`, `chore` and `documentation`. See [PR Labeler settings](.github/pr-labeler.yml) for automatically labeling from the branch name.


## Release workflow

Run `Actions | Create release pull request | Run workflow` and merge created PR.
(Or bump version in package.json on default branch)

Then, Will do the release process automatically by [Release](.github/workflows/release.yml) workflow.
And after tagged, OpenUPM retrieves the tag and updates it.

Do **NOT** manually operation the following operations:

- Create release tag
- Publish draft releases
