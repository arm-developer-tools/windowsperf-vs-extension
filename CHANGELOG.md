# Changelogs

## [v1.0.0-beta..v1.0.0](https://gitlab.com/Linaro/WindowsPerf/vs-extension/compare/1.0.0-beta...v1.0.0) (2023-09-18)

### Feat

- bump version to 1.0.0
- change search engin to be use a linear algorithm rather than a fuzzy searcher
- add UI for autocomplete and search algorithm
- show default frequency from wperf test
- use absolute overhead if root is clicked
- add hints for highlighted lines
- inital addition of sampling manager option page
- handle event hit collision on same line number
- WPERF-683 add hints for highlighted lines
- WPERF-696 disassembly highlight color depends on overhead
- WPERF-484 Add raw events support
- raw events error handling/delete raw events
- WPERF-697 Add sampling coverage percentage event level
- add sampling overhead at root level
- print 0xF00F00 and 0xFEEFEE as hex
- WPERF-698 Code cleanup and monospace font integration
- add scroll to highlited text
- add text highlighting
- add disassembly to sampling section formatting
- add disassembly argument to wperf command
- add disassembly parsing
- add color highlighting for sampling feature
- update filePath when project target is selected
- make file picker disabled when project target is selected
- clump UI elements that are related
- add support for successive event deletion
- finalize design and update language packs
- add support for event addition and deletion
- UI improvement and disable duplicated events
- bare bones design for the event listbox
- add sampling listbox logic
- add localization to options page
- add localization for Cpu cores
- add support for localization for sampling explorer
- add language support for windwosPerf Host data
- localize SamplingSettingsDialog
- translate error list
- **i18n** setup localization for the project
- **theming** sync all colors to visual studio colors
- **theming** add support for vs colors
- update color schema
- add double click to expand feature to sampling explorer list
- allign numbers to the right
- add line number to sampling explorer list
- update sampling explorer UI
- **sample-UI** update sampling ui buttons
- sampling UI padding update and error hanling
- implement logic for sampling explorer
- **sampling** deserialize sampling output
- add logic to saving changes on the settings class
- close window onSave
- add logic to settings dialog
- **sampling-explorer** add top toolbar to sampling explorer and known moniker buttons
- Sampling settings UI
- customRadio Button control
- add customComboBoxControl
- add label to FilePicker
- add custom button
- create custom TextBox control
- successful custom contorl
- custom button and file picker
- layout blueprint
- add cpu core list fetching
- **wperf-output** add output log on every wperf command
- add test project with sample scenario
- **wperf-list-ui** add event list and metric list to option UI
- **wperf-list** add suport for details flag
- **wperf-test-ui** refine UI
- **wperf-test** add test ui for wperf
- **version-ui** add version verification in settings UI
- **wperf-list** add support for wperf list command
- **options** add option to users to manually setting wperf path

### Fix

- update tests and fix CI
- typo in psh cmd
- realign scrollviewer section
- remove unnecessary check in pdbFilePath assignment
- sampling form validation bug
- update UI button and groupbox
- update sampling script event param
- portuguese translation
- refactor folder structure for tool windows
- revert a comment on the predefined events
- update typo in english language
- update language packs access level
- revert commented code to original state
- **SamplingSettings** move PROCESS and ARGS at the end of wperf record command line
- rename windows
- **sampling-explorer** refine UI for sampling
- update sampling preview command to record
- update UI for textbox
- **sampling-layout** registring windows
- **wperf-list-ui** wrap the page in a scrollViewer
- **wperf-test-ui** close dialogWindow when error is captured on wperf
- **wperf-version-ui** issue with double rendered UI
- **build** update build targets and add support for x86
- **wperf-version-ui** persist init data
- **wperf-version-ui** arragne styling
- **wperf-client** fix typo in GetEventList summary
- **docs** clarify WperfTest Class short description

### Docs

- update wiki link and contribution
- **README** add how to install and where is release
- **sampling-explorer** update resources for sampling explorer
- samploing UI demo
- add sampling explorer screen recordings
- add a line in main readme for wperf output window
- **wperf-list-ui** update umentation for the settings ui
- **wperf-test-ui** add title to the refrences
- **wperf-test-ui** fix typos and add refs to
- **wperf-extension** add installation steps for the vsix
- **wperf-test-ui** add tutorial for how to check host data
- **wperf-test-ui** add screenshotos to wperf additional host data
- update initilization seteps
- **wperf-outputs** update typos in wperfOutput
- **wperf-list** add for the wperf list command
- **options** add option setup tutorial in the main README.md file

### CI

- WPERF-791 - Fix CI failing on the test stage
- Change test project output type to `exe` and update ci accordingly.
- Include build CI natively on ARM64

### Tests

- add unit tests to Fuzzy searcher

### Merge Requests

- [WPERF-462 feat: bump version to 1.0.0](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/62)
- [WPERF-483 : Autocomplete component](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/60)
- [feat: WPERF-601 show default frequency from wperf test](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/61)
- [WPERF-774 chore: update translation strings + fix raw event regex](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/59)
- [feat: WPERF-783 Heatmap use absolute overhead in root](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/57)
- [ci: WPERF-791 - Fix CI failing on the test stage](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/58)
- [WPERF-754 Option for color resolution](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/55)
- [feat: WPERF-683 add hints for highlighted lines](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/54)
- [feat: WPERF-770 handle event hit collision on same line number](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/56)
- [feat: WPERF-696 disassembly highlight color depends on overhead](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/53)
- [feat: WPERF-484 Add raw events support](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/52)
- [feat: WPERF-697 Add sampling coverage percentage event level](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/51)
- [feat: WPERF-699 print 0xF00F00 and 0xFEEFEE as hex](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/50)
- [feat: WPERF-698 Code cleanup and monospace font integration](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/49)
- [WPERF-591 Disassembly feature](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/48)
- [WPERF-592 feat: add color highlighting for sampling feature](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/47)
- [WPERF-632 Sampling settings ui refactor](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/46)
- [WPERF-605 Support for double dashs](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/45)
- [WPERF-615 Support path whitespaces](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/44)
- [WPERF-479 Select project target](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/43)
- [WPERF-588 update wiki link and contribution](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/42)
- [WPERF-482 Multiple events support](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/41)
- [Remove docs](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/40)
- [Update docs](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/39)
- [i18n: adding both Portuguese and Polish](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/38)
- [Introducing i18n](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/37)
- [Remove commented code](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/36)
- [fix: revert commented code to original state](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/35)
- [Refactor part (1)](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/34)
- [Add Articles section for blog links](https://gitlab.com/Linaro/WindowsPerf/vs-extension/-/merge_requests/33)
