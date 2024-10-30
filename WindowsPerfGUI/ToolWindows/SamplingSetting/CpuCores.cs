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

using System.Collections.Generic;
using WindowsPerfGUI.Resources.Locals;

namespace WindowsPerfGUI.ToolWindows.SamplingSetting
{
  public class CpuCoreElement
  {
    public int coreNumber;
    public IntPtr coreMask;

    public override string ToString()
    {
      return $"{SamplingSettingsLanguagePack.CpuCoreNumber} {coreNumber}";
    }
  }

  public static class CpuCores
  {
    public static int numberOfAvailableCores = 0;

    public static List<CpuCoreElement> CpuCoreList { get; private set; }

    public static List<CpuCoreElement> InitCpuCores()
    {
      if (numberOfAvailableCores > 0)
        return CpuCoreList;
      foreach (
          var item in new System.Management.ManagementObjectSearcher(
              "Select * from Win32_Processor"
          ).Get()
      )
      {
        numberOfAvailableCores += int.Parse(item["NumberOfCores"].ToString());
      }
      CreateCpuCoreList();
      return CpuCoreList;
    }

    private static void CreateCpuCoreList()
    {
      CpuCoreList = new List<CpuCoreElement>();
      for (int i = 0; i < numberOfAvailableCores; i++)
      {
        CpuCoreList.Add(
            new CpuCoreElement { coreNumber = i, coreMask = (IntPtr)(0x1 << i) }
        );
      }
    }
  }
}
