// BSD 3-Clause License
//
// Copyright (c) 2024, Arm Limited
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its
//    contributors may be used to endorse or promote products derived from
//    this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using WindowsPerfGUI.Options;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.ToolWindows.SamplingExplorer;
using WindowsPerfGUI.Utils;

namespace WindowsPerfGUI.Commands
{
  [Command(PackageIds.SamplingExplorer)]
  internal sealed class SamplingExplorerCommand : BaseCommand<SamplingExplorerCommand>
  {
    protected override void BeforeQueryStatus(EventArgs e)
    {
      string windowTitle = SamplingExplorerLanguagePack.WindowTitle;
      try
      {
        if (
            WPerfOptions.Instance.WperfCurrentVersion != null
            && WPerfOptions.Instance.WperfCurrentVersion.Components[0].ComponentVersion
            != WperfDefaults.WPERF_MIN_VERSION
        )
          windowTitle += $" - (UNSTABLE)";
      }
      catch (Exception) { }
      Command.Text = windowTitle;
      base.BeforeQueryStatus(e);
    }

    protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
      if (!WPerfOptions.Instance.IsWperfInitialized)
      {
        await VS.MessageBox.ShowErrorAsync(
            ErrorLanguagePack.NotInititiatedWperfErrorLine1,
            ErrorLanguagePack.NotInititiatedWperfErrorLine2
        );
        return;
      }

      if (
          WPerfOptions.Instance.WperfCurrentVersion.Components[0].ComponentVersion
          != WperfDefaults.WPERF_MIN_VERSION
      )
      {
        if (WPerfOptions.Instance.WperfVersionCheckIgnore != true)
        {
          await VS.MessageBox.ShowErrorAsync(
              string.Format(
                  ErrorLanguagePack.MinimumVersionMismatch,
                  WperfDefaults.WPERF_MIN_VERSION
              ),
              ErrorLanguagePack.MinimumVersionMismatchLine2
          );
          return;
        }
        var messageBoxResult = await VS.MessageBox.ShowWarningAsync(
            string.Format(
                ErrorLanguagePack.MinimumVersionMismatch,
                WperfDefaults.WPERF_MIN_VERSION
            )
        );
        if (
            messageBoxResult == Microsoft.VisualStudio.VSConstants.MessageBoxResult.IDCANCEL
        )
        {
          return;
        }
      }
      await SamplingExplorer.ShowAsync();
    }
  }
}
