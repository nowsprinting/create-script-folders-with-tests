# Create Script Folders and Assemblies with Tests

[![Meta file check](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/metacheck.yml/badge.svg)](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/metacheck.yml)
[![Test](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/test.yml/badge.svg)](https://github.com/nowsprinting/create-script-folders-with-tests/actions/workflows/test.yml)
[![openupm](https://img.shields.io/npm/v/com.nowsprinting.create-script-folders-with-tests?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.nowsprinting.create-script-folders-with-tests/)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/nowsprinting/create-script-folders-with-tests)

This Unity editor extension creates script folders (Editor, Runtime, and each Tests) containing assembly definition files (.asmdef).


## Features

When opening the context menu and selecting
**Create > C# Script Folders and Assemblies with Tests**
The root folder (e.g., named **YourFeature**) and all folders below it will be created as follows:

### Creating folders and asmdefs

#### Using under Assets folder

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

#### Using under Packages folder

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

> [!IMPORTANT]  
> Package folder (e.g., named **your.package.name**) and package.json must be created before.
> Because you can not open the context menu directly under the Packages folder.

After creating folders, move the Editor, Runtime, and Tests folders directly under the **your.package.name** folder.
And remove the **YourFeature** folder.
Then it will be the same as the official [package layout](https://docs.unity3d.com/Manual/cus-layout.html).

```
Packages
└── your.package.name
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

> [!WARNING]  
> Move folders using JetBrains Rider.
> Because to update DotSettings files (see below).


### Assembly Definition References in asmdefs

**Assembly Definition References** in each asmdef are set as follows:

```mermaid
graph RL
    Runtime
    Editor --> Runtime
    Runtime.Tests --> Runtime
    Editor.Tests --> Runtime
    Editor.Tests --> Editor
```


### Creating DotSettings files

And creating .csproj.DotSettings file for each assembly.
This file is set up to make the [Namespace does not correspond to file location](https://www.jetbrains.com/help/rider/CheckNamespace.html) inspection work as expected in JetBrains Rider.
Do not forget to commit .DotSettings files for that project.

Specifically, disabled the [Namespace provider](https://www.jetbrains.com/help/rider/Refactorings__Adjust_Namespaces.html) for the following folders:

- Scripts
- Scripts/Runtime
- Tests
- Tests/Runtime

This will result in the expected namespace per folder as follows:

- Scripts/Editor: `YourFeature.Editor`
- Scripts/Runtime: `YourFeature`
- Tests/Editor: `YourFeature.Editor`
- Tests/Runtime: `YourFeature`

> [!WARNING]  
> Under Packages namespace resolution works with Unity 2020.2 or later.
> Because to use the Root Namespace property of asmdef.

See also: [Code Inspections in C# | JetBrains Rider Documentation](https://www.jetbrains.com/help/rider/Reference__Code_Inspections_CSHARP.html)


## Installation

1. Open the Project Settings window (**Editor > Project Settings**) and select **Package Manager** tab (figure 1.)
2. Click **+** button under the **Scoped Registries** and enter the following settings:
    1. **Name:** `package.openupm.com`
    2. **URL:** `https://package.openupm.com`
    3. **Scope(s):** `com.nowsprinting`
3. Open the Package Manager window (**Window > Package Manager**) and select **My Registries** tab (figure 2.)
4. Select **Create Script Folders and Assemblies with Tests** and click the **Install** button

**Figure 1.** Scoped Registries setting in Project Settings window

![](Documentation~/ScopedRegistries_Dark.png#gh-dark-mode-only)
![](Documentation~/ScopedRegistries_Light.png#gh-light-mode-only)

**Figure 2.** My Registries in Package Manager window

![](Documentation~/PackageManager_Dark.png#gh-dark-mode-only)
![](Documentation~/PackageManager_Light.png#gh-light-mode-only)


## License

MIT License


## How to contribute

Open an issue or create a pull request.

Be grateful if you could label the pull request as `enhancement`, `bug`, `chore`, and `documentation`. See [PR Labeler settings](.github/pr-labeler.yml) for automatically labeling from the branch name.


## How to development

### Clone repo as a embedded package

Add this repository as a submodule to the Packages/ directory in your project.

Run the command below:

```bash
git submodule add git@github.com:nowsprinting/create-script-folders-with-tests.git Packages/com.nowsprinting.create-script-folders-with-tests
```


### Run tests

Generate a temporary project and run tests on each Unity version from the command line.

```bash
make create_project
UNITY_VERSION=2019.4.40f1 make -k test
```


### Release workflow

The release process is as follows:

1. Run **Actions > Create release pull request > Run workflow**
2. Merge created pull request

Then, will do the release process automatically by [Release](.github/workflows/release.yml) workflow.
After tagging, [OpenUPM](https://openupm.com/) retrieves the tag and updates it.

> [!CAUTION]  
> Do **NOT** manually operation the following operations:
> - Create a release tag
> - Publish draft releases

> [!CAUTION]  
> You must modify the package name to publish a forked package.
