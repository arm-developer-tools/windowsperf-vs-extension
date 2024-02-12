// BSD 3-Clause License
//
// Copyright (c) 2022, Arm Limited
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

using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using WindowsPerfGUI.Components.TreeListView;
using WindowsPerfGUI.Options;
using WindowsPerfGUI.Resources.Locals;
using WindowsPerfGUI.SDK;
using WindowsPerfGUI.SDK.WperfOutputs;
using WindowsPerfGUI.ToolWindows.SamplingExplorer.LineHighlighting;
using WindowsPerfGUI.ToolWindows.SamplingSetting;

namespace WindowsPerfGUI.ToolWindows.SamplingExplorer
{
    public partial class SamplingExplorerControl : UserControl
    {
        readonly WperfClientFactory wperfClient = new();
        static FormattedSamplingResults formattedSamplingResults = new FormattedSamplingResults();
#if DEBUG
        string rootSample =
            "{\r\n\"sampling\": {\r\n\"pe_file\": \"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\ARM64\\\\Debug\\\\WindowsPerfSample.exe\",\r\n\"pdb_file\": \"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\ARM64\\\\Debug\\\\WindowsPerfSample.pdb\",\r\n\"sample_display_row\": 50,\r\n\"samples_generated\": 3681,\r\n\"samples_dropped\": 0,\r\n\"base_address\": 140698715427836,\r\n\"runtime_delta\": 140693346648064,\r\n\"modules\":[{\"name\":\"KERNEL32.DLL\",\"address\":140711040712704,\"path\":\"C:\\\\Windows\\\\System32\\\\KERNEL32.DLL\"},{\"name\":\"KERNELBASE.dll\",\"address\":140710975504384,\"path\":\"C:\\\\Windows\\\\System32\\\\KERNELBASE.dll\"},{\"name\":\"VCRUNTIME140D.dll\",\"address\":140710480904192,\"path\":\"C:\\\\Windows\\\\SYSTEM32\\\\VCRUNTIME140D.dll\"},{\"name\":\"WindowsPerfSample.exe\",\"address\":140698715357184,\"path\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\ARM64\\\\Debug\\\\WindowsPerfSample.exe\"},{\"name\":\"ntdll.dll\",\"address\":140711053623296,\"path\":\"C:\\\\Windows\\\\SYSTEM32\\\\ntdll.dll\"},{\"name\":\"ucrtbased.dll\",\"address\":140708302815232,\"path\":\"C:\\\\Windows\\\\SYSTEM32\\\\ucrtbased.dll\"}],\r\n\"modules_info\": [{\"sections\":[{\"section\":\".textbss\",\"offset\":4096,\"virtual_size\":65536},{\"section\":\".text\",\"offset\":69632,\"virtual_size\":30701},{\"section\":\".rdata\",\"offset\":102400,\"virtual_size\":6791},{\"section\":\".data\",\"offset\":110592,\"virtual_size\":640},{\"section\":\".pdata\",\"offset\":114688,\"virtual_size\":5360},{\"section\":\".idata\",\"offset\":122880,\"virtual_size\":2559},{\"section\":\".msvcjmc\",\"offset\":126976,\"virtual_size\":275},{\"section\":\".00cfg\",\"offset\":131072,\"virtual_size\":337},{\"section\":\".rsrc\",\"offset\":135168,\"virtual_size\":1084},{\"section\":\".reloc\",\"offset\":139264,\"virtual_size\":431}],\"module\":\"WindowsPerfSample.exe\",\"pdb_file\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\ARM64\\\\Debug\\\\WindowsPerfSample.pdb\",\"pe_name\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\ARM64\\\\Debug\\\\WindowsPerfSample.exe\"}],\r\n\"events\": [{\"type\":\"ld_spec\",\"samples\":[{\"overhead\":98.176,\"count\":1830,\"symbol\":\"simd_hot\"},{\"overhead\":1.07296,\"count\":20,\"symbol\":\"unknown\"},{\"overhead\":0.375536,\"count\":7,\"symbol\":\"df_hot\"},{\"overhead\":0.214592,\"count\":4,\"symbol\":\"__CheckForDebuggerJustMyCode\"},{\"overhead\":0.160944,\"count\":3,\"symbol\":\"main\"}],\"interval\":67108864,\"printed_sample_num\":5,\r\n\"pcs\":[{\"address\":140698715435224,\"count\":264},{\"address\":140698715435260,\"count\":263},{\"address\":140698715435228,\"count\":223},{\"address\":140698715435268,\"count\":198},{\"address\":140698715435220,\"count\":139},{\"address\":140698715435240,\"count\":138},{\"address\":140698715435248,\"count\":137},{\"address\":140698715435256,\"count\":130},{\"address\":140698715435216,\"count\":56},{\"address\":140698715435264,\"count\":44},{\"address\":207898992,\"count\":1},{\"address\":140711053947600,\"count\":1},{\"address\":140711055154100,\"count\":1},{\"address\":203201520,\"count\":1},{\"address\":204812672,\"count\":1},{\"address\":248454380,\"count\":1},{\"address\":140710880292308,\"count\":1},{\"address\":3186730430516,\"count\":1},{\"address\":2005211756,\"count\":1},{\"address\":2244302915868,\"count\":1},{\"address\":140698715434948,\"count\":5},{\"address\":140698715435052,\"count\":1},{\"address\":140698715434956,\"count\":1},{\"address\":140698715436144,\"count\":2},{\"address\":140698715436156,\"count\":1},{\"address\":140698715436116,\"count\":1},{\"address\":140698715435932,\"count\":1},{\"address\":140698715435944,\"count\":1},{\"address\":140698715435916,\"count\":1}],\r\n\"annotate\": [{\"function_name\": \"simd_hot\",\r\n\"source_code\":[{\"line_number\":53,\"hits\":1635,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"130d8\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"130d8\",\"instruction\":\"ldr     q18, [x19], #0x40\"},{\"address\":\"130dc\",\"instruction\":\"add       v16.4s, v16.4s, v18.4s\"},{\"address\":\"130e0\",\"instruction\":\"stur q16, [x9, #-0x20]\"},{\"address\":\"130e4\",\"instruction\":\"ldr       q16, [x13, x8]\"},{\"address\":\"130e8\",\"instruction\":\"add  v16.4s, v16.4s, v17.4s\"},{\"address\":\"130ec\",\"instruction\":\"ldr  q17, [x11, x9]\"},{\"address\":\"130f0\",\"instruction\":\"str  q16, [x12, x8]\"},{\"address\":\"130f4\",\"instruction\":\"ldp  q16, q18, [x8, #0x10]\"},{\"address\":\"130f8\",\"instruction\":\"add   x8, x8, #0x40\"},{\"address\":\"130fc\",\"instruction\":\"add   v17.4s, v17.4s, v16.4s\"},{\"address\":\"13100\",\"instruction\":\"ldur q16, [x19, #-0x10]\"},{\"address\":\"13104\",\"instruction\":\"add      v16.4s, v18.4s, v16.4s\"},{\"address\":\"13108\",\"instruction\":\"stp q17, q16, [x9], #0x40\"},{\"address\":\"1310c\",\"instruction\":\"cbnz   w10, 0x1400130d0 <.text+0x20d0>\"},{\"address\":\"13110\",\"instruction\":\"ldp x29, x30, [sp], #0x10\"},{\"address\":\"13114\",\"instruction\":\"ldr   x21, [sp, #0x10]\"},{\"address\":\"13118\",\"instruction\":\"ldp        x19, x20, [sp], #0x20\"},{\"address\":\"1311c\",\"instruction\":\"ret\"}]}},{\"line_number\":52,\"hits\":195,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"130d4\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"130d0\",\"instruction\":\"ldp  q16, q17, [x8, #-0x10]\"},{\"address\":\"130d4\",\"instruction\":\"sub w10, w10, #0x1\"}]}}]},\r\n{\"function_name\": \"df_hot\",\r\n\"source_code\":[{\"line_number\":15732480,\"hits\":6,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"12fc4\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"12fb0\",\"instruction\":\"adrp x8, 0x14001f000\"},{\"address\":\"12fb4\",\"instruction\":\"add x0, x8, #0x0\"},{\"address\":\"12fb8\",\"instruction\":\"bl     0x140011594 <.text+0x594>\"},{\"address\":\"12fbc\",\"instruction\":\"adrp      x9, 0x14001b000\"},{\"address\":\"12fc0\",\"instruction\":\"ldr d16, [x9]\"},{\"address\":\"12fc4\",\"instruction\":\"ldr       d18, 0x140013030 <.text+0x2030>\"},{\"address\":\"12fc8\",\"instruction\":\"ldr d17, 0x140013038 <.text+0x2038>\"},{\"address\":\"12fcc\",\"instruction\":\"fdiv        d16, d16, d18\"},{\"address\":\"12fd0\",\"instruction\":\"fadd  d19, d16, d17\"},{\"address\":\"12fd4\",\"instruction\":\"ldr   d16, 0x140013040 <.text+0x2040>\"},{\"address\":\"12fd8\",\"instruction\":\"fmul        d0, d19, d16\"},{\"address\":\"12fdc\",\"instruction\":\"str    d0, [x9]\"},{\"address\":\"12fe0\",\"instruction\":\"cbnz       w19, 0x140013024 <.text+0x2024>\"}]}},{\"line_number\":45,\"hits\":1,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"1302c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"12ff4\",\"instruction\":\"fmul d19, d19, d16\"},{\"address\":\"12ff8\",\"instruction\":\"fdiv  d19, d19, d18\"},{\"address\":\"12ffc\",\"instruction\":\"fadd  d19, d19, d17\"},{\"address\":\"13000\",\"instruction\":\"fmul  d19, d19, d16\"},{\"address\":\"13004\",\"instruction\":\"fdiv d19, d19, d18\"},{\"address\":\"13008\",\"instruction\":\"fadd   d19, d19, d17\"},{\"address\":\"1300c\",\"instruction\":\"fmul  d19, d19, d16\"},{\"address\":\"13010\",\"instruction\":\"fdiv  d19, d19, d18\"},{\"address\":\"13014\",\"instruction\":\"fadd  d19, d19, d17\"},{\"address\":\"13018\",\"instruction\":\"fmul  d0, d19, d16\"},{\"address\":\"1301c\",\"instruction\":\"str    d0, [x9]\"},{\"address\":\"13020\",\"instruction\":\"cbnz       w8, 0x140012fe8 <.text+0x1fe8>\"},{\"address\":\"13024\",\"instruction\":\"ldp  x29, x30, [sp], #0x10\"},{\"address\":\"13028\",\"instruction\":\"ldr   x19, [sp], #0x10\"},{\"address\":\"1302c\",\"instruction\":\"ret\"},{\"address\":\"13030\",\"instruction\":\"<unknown>\"},{\"address\":\"13034\",\"instruction\":\"<unknown>\"},{\"address\":\"13038\",\"instruction\":\"<unknown>\"},{\"address\":\"1303c\",\"instruction\":\"<unknown>\"},{\"address\":\"13040\",\"instruction\":\"ldur       h18, [x5, #-0x3d]\"},{\"address\":\"13044\",\"instruction\":\"<unknown>\"}]}}]},\r\n{\"function_name\": \"__CheckForDebuggerJustMyCode\",\r\n\"source_code\":[{\"line_number\":25,\"hits\":3,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\",\"instruction_address\":\"13470\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13468\",\"instruction\":\"ldr     x8, [sp, #0x10]\"},{\"address\":\"1346c\",\"instruction\":\"ldrb w8, [x8]\"},{\"address\":\"13470\",\"instruction\":\"mov        w8, w8\"},{\"address\":\"13474\",\"instruction\":\"cmp  w8, #0x0\"},{\"address\":\"13478\",\"instruction\":\"b.eq       0x1400134b0 <.text+0x24b0>\"},{\"address\":\"1347c\",\"instruction\":\"adrp     x8, 0x14001b000\"},{\"address\":\"13480\",\"instruction\":\"ldr w8, [x8, #0x274]\"},{\"address\":\"13484\",\"instruction\":\"cmp        w8, #0x0\"},{\"address\":\"13488\",\"instruction\":\"b.eq       0x1400134b0 <.text+0x24b0>\"},{\"address\":\"1348c\",\"instruction\":\"adrp     x8, 0x14001e000\"},{\"address\":\"13490\",\"instruction\":\"ldr x8, [x8, #0x30]\"},{\"address\":\"13494\",\"instruction\":\"blr x8\"},{\"address\":\"13498\",\"instruction\":\"mov      w9, w0\"},{\"address\":\"1349c\",\"instruction\":\"adrp x8, 0x14001b000\"},{\"address\":\"134a0\",\"instruction\":\"ldr w8, [x8, #0x274]\"},{\"address\":\"134a4\",\"instruction\":\"cmp        w8, w9\"},{\"address\":\"134a8\",\"instruction\":\"b.ne 0x1400134b0 <.text+0x24b0>\"}]}},{\"line_number\":22,\"hits\":1,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\",\"instruction_address\":\"13454\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13450\",\"instruction\":\"pacibsp\"},{\"address\":\"13454\",\"instruction\":\"stp x29, x30, [sp, #-0x20]!\"},{\"address\":\"13458\",\"instruction\":\"mov x29, sp\"},{\"address\":\"1345c\",\"instruction\":\"strx0, [sp, #0x10]\"}]}}]},\r\n{\"function_name\": \"main\",\r\n\"source_code\":[{\"line_number\":64,\"hits\":2,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\main.c\",\"instruction_address\":\"1339c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"1339c\",\"instruction\":\"mov       x2, x20\"},{\"address\":\"133a0\",\"instruction\":\"mov x1, x21\"},{\"address\":\"133a4\",\"instruction\":\"mov x0, x22\"},{\"address\":\"133a8\",\"instruction\":\"bl  0x1400115d0 <.text+0x5d0>\"},{\"address\":\"133ac\",\"instruction\":\"sub       w19, w19, #0x1\"},{\"address\":\"133b0\",\"instruction\":\"cbnz w19, 0x14001339c <.text+0x239c>\"}]}},{\"line_number\":62,\"hits\":1,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\main.c\",\"instruction_address\":\"1338c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13384\",\"instruction\":\"fmov        d0, d8\"},{\"address\":\"13388\",\"instruction\":\"mov  w0, #0x1\"},{\"address\":\"1338c\",\"instruction\":\"bl0x1400111d4 <.text+0x1d4>\"},{\"address\":\"13390\",\"instruction\":\"sub        w19, w19, #0x1\"},{\"address\":\"13394\",\"instruction\":\"cbnz w19, 0x140013384 <.text+0x2384>\"}]}}]}]}\r\n,\r\n{\"type\":\"vfp_spec\",\"samples\":[{\"overhead\":55.0358,\"count\":1000,\"symbol\":\"simd_hot\"},{\"overhead\":28.6736,\"count\":521,\"symbol\":\"df_hot\"},{\"overhead\":11.0072,\"count\":200,\"symbol\":\"unknown\"},{\"overhead\":4.40286,\"count\":80,\"symbol\":\"__CheckForDebuggerJustMyCode\"},{\"overhead\":0.880572,\"count\":16,\"symbol\":\"main\"}],\"interval\":80000,\"printed_sample_num\":5,\r\n\"pcs\":[{\"address\":140698715435224,\"count\":160},{\"address\":140698715435260,\"count\":147},{\"address\":140698715435228,\"count\":111},{\"address\":140698715435268,\"count\":110},{\"address\":140698715435256,\"count\":80},{\"address\":140698715435220,\"count\":76},{\"address\":140698715435248,\"count\":74},{\"address\":140698715435240,\"count\":73},{\"address\":140698715435216,\"count\":32},{\"address\":140698715435272,\"count\":27},{\"address\":140698715434948,\"count\":403},{\"address\":140698715434916,\"count\":15},{\"address\":140698715435048,\"count\":14},{\"address\":140698715434920,\"count\":12},{\"address\":140698715434940,\"count\":9},{\"address\":140698715434952,\"count\":9},{\"address\":140698715434944,\"count\":8},{\"address\":140698715434928,\"count\":7},{\"address\":140698715434964,\"count\":6},{\"address\":140698715435052,\"count\":6},{\"address\":140698715427284,\"count\":9},{\"address\":140698715428244,\"count\":8},{\"address\":140698715427292,\"count\":8},{\"address\":140698715428248,\"count\":4},{\"address\":3186748683680,\"count\":2},{\"address\":140710880275256,\"count\":2},{\"address\":140698715428252,\"count\":2},{\"address\":255796204,\"count\":1},{\"address\":140710880274228,\"count\":1},{\"address\":140710976848052,\"count\":1},{\"address\":140698715436132,\"count\":11},{\"address\":140698715436156,\"count\":8},{\"address\":140698715436124,\"count\":8},{\"address\":140698715436116,\"count\":7},{\"address\":140698715436212,\"count\":7},{\"address\":140698715436160,\"count\":6},{\"address\":140698715436128,\"count\":6},{\"address\":140698715436216,\"count\":6},{\"address\":140698715436164,\"count\":5},{\"address\":140698715436112,\"count\":5},{\"address\":140698715435908,\"count\":7},{\"address\":140698715435920,\"count\":5},{\"address\":140698715435916,\"count\":3},{\"address\":140698715435932,\"count\":1}],\r\n\"annotate\": [{\"function_name\": \"simd_hot\",\r\n\"source_code\":[{\"line_number\":53,\"hits\":889,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"130d8\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"130d8\",\"instruction\":\"ldr      q18, [x19], #0x40\"},{\"address\":\"130dc\",\"instruction\":\"add       v16.4s, v16.4s, v18.4s\"},{\"address\":\"130e0\",\"instruction\":\"stur q16, [x9, #-0x20]\"},{\"address\":\"130e4\",\"instruction\":\"ldr       q16, [x13, x8]\"},{\"address\":\"130e8\",\"instruction\":\"add  v16.4s, v16.4s, v17.4s\"},{\"address\":\"130ec\",\"instruction\":\"ldr  q17, [x11, x9]\"},{\"address\":\"130f0\",\"instruction\":\"str  q16, [x12, x8]\"},{\"address\":\"130f4\",\"instruction\":\"ldp  q16, q18, [x8, #0x10]\"},{\"address\":\"130f8\",\"instruction\":\"add   x8, x8, #0x40\"},{\"address\":\"130fc\",\"instruction\":\"add   v17.4s, v17.4s, v16.4s\"},{\"address\":\"13100\",\"instruction\":\"ldur q16, [x19, #-0x10]\"},{\"address\":\"13104\",\"instruction\":\"add      v16.4s, v18.4s, v16.4s\"},{\"address\":\"13108\",\"instruction\":\"stp q17, q16, [x9], #0x40\"},{\"address\":\"1310c\",\"instruction\":\"cbnz   w10, 0x1400130d0 <.text+0x20d0>\"},{\"address\":\"13110\",\"instruction\":\"ldp x29, x30, [sp], #0x10\"},{\"address\":\"13114\",\"instruction\":\"ldr   x21, [sp, #0x10]\"},{\"address\":\"13118\",\"instruction\":\"ldp        x19, x20, [sp], #0x20\"},{\"address\":\"1311c\",\"instruction\":\"ret\"}]}},{\"line_number\":52,\"hits\":108,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"130d4\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"130d0\",\"instruction\":\"ldp  q16, q17, [x8, #-0x10]\"},{\"address\":\"130d4\",\"instruction\":\"sub w10, w10, #0x1\"}]}},{\"line_number\":15732480,\"hits\":3,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"130b0\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"130a4\",\"instruction\":\"adrp     x8, 0x14001f000\"},{\"address\":\"130a8\",\"instruction\":\"add x0, x8, #0x0\"},{\"address\":\"130ac\",\"instruction\":\"mov    x20, x1\"},{\"address\":\"130b0\",\"instruction\":\"mov x19, x2\"},{\"address\":\"130b4\",\"instruction\":\"bl  0x140011594 <.text+0x594>\"},{\"address\":\"130b8\",\"instruction\":\"add       x9, x21, #0x20\"},{\"address\":\"130bc\",\"instruction\":\"add  x8, x20, #0x10\"},{\"address\":\"130c0\",\"instruction\":\"sub  x13, x19, x20\"},{\"address\":\"130c4\",\"instruction\":\"sub   x12, x21, x20\"},{\"address\":\"130c8\",\"instruction\":\"sub   x11, x19, x21\"},{\"address\":\"130cc\",\"instruction\":\"mov   w10, #0x271\"}]}}]},\r\n{\"function_name\": \"df_hot\",\r\n\"source_code\":[{\"line_number\":15732480,\"hits\":466,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"12fc4\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"12fb0\",\"instruction\":\"adrp       x8, 0x14001f000\"},{\"address\":\"12fb4\",\"instruction\":\"add x0, x8, #0x0\"},{\"address\":\"12fb8\",\"instruction\":\"bl     0x140011594 <.text+0x594>\"},{\"address\":\"12fbc\",\"instruction\":\"adrp      x9, 0x14001b000\"},{\"address\":\"12fc0\",\"instruction\":\"ldr d16, [x9]\"},{\"address\":\"12fc4\",\"instruction\":\"ldr       d18, 0x140013030 <.text+0x2030>\"},{\"address\":\"12fc8\",\"instruction\":\"ldrd17, 0x140013038 <.text+0x2038>\"},{\"address\":\"12fcc\",\"instruction\":\"fdiv d16, d16, d18\"},{\"address\":\"12fd0\",\"instruction\":\"fadd  d19, d16, d17\"},{\"address\":\"12fd4\",\"instruction\":\"ldr   d16, 0x140013040 <.text+0x2040>\"},{\"address\":\"12fd8\",\"instruction\":\"fmul        d0, d19, d16\"},{\"address\":\"12fdc\",\"instruction\":\"str    d0, [x9]\"},{\"address\":\"12fe0\",\"instruction\":\"cbnz       w19, 0x140013024 <.text+0x2024>\"}]}},{\"line_number\":33,\"hits\":31,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"12fa4\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"12fa0\",\"instruction\":\"str x19, [sp, #-0x10]!\"},{\"address\":\"12fa4\",\"instruction\":\"stp      x29, x30, [sp, #-0x10]!\"},{\"address\":\"12fa8\",\"instruction\":\"mov x29, sp\"},{\"address\":\"12fac\",\"instruction\":\"mov w19, w0\"}]}},{\"line_number\":45,\"hits\":24,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\lib.c\",\"instruction_address\":\"13028\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"12ff4\",\"instruction\":\"fmul        d19, d19, d16\"},{\"address\":\"12ff8\",\"instruction\":\"fdiv  d19, d19, d18\"},{\"address\":\"12ffc\",\"instruction\":\"fadd  d19, d19, d17\"},{\"address\":\"13000\",\"instruction\":\"fmul  d19, d19, d16\"},{\"address\":\"13004\",\"instruction\":\"fdiv  d19, d19, d18\"},{\"address\":\"13008\",\"instruction\":\"fadd  d19, d19, d17\"},{\"address\":\"1300c\",\"instruction\":\"fmul  d19, d19, d16\"},{\"address\":\"13010\",\"instruction\":\"fdiv  d19, d19, d18\"},{\"address\":\"13014\",\"instruction\":\"fadd  d19, d19, d17\"},{\"address\":\"13018\",\"instruction\":\"fmul  d0, d19, d16\"},{\"address\":\"1301c\",\"instruction\":\"str    d0, [x9]\"},{\"address\":\"13020\",\"instruction\":\"cbnz       w8, 0x140012fe8 <.text+0x1fe8>\"},{\"address\":\"13024\",\"instruction\":\"ldp  x29, x30, [sp], #0x10\"},{\"address\":\"13028\",\"instruction\":\"ldr   x19, [sp], #0x10\"},{\"address\":\"1302c\",\"instruction\":\"ret\"},{\"address\":\"13030\",\"instruction\":\"<unknown>\"},{\"address\":\"13034\",\"instruction\":\"<unknown>\"},{\"address\":\"13038\",\"instruction\":\"<unknown>\"},{\"address\":\"1303c\",\"instruction\":\"<unknown>\"},{\"address\":\"13040\",\"instruction\":\"ldur       h18, [x5, #-0x3d]\"},{\"address\":\"13044\",\"instruction\":\"<unknown>\"}]}}]},\r\n{\"function_name\": \"__CheckForDebuggerJustMyCode\",\r\n\"source_code\":[{\"line_number\":25,\"hits\":27,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\",\"instruction_address\":\"1347c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13468\",\"instruction\":\"ldr    x8, [sp, #0x10]\"},{\"address\":\"1346c\",\"instruction\":\"ldrb w8, [x8]\"},{\"address\":\"13470\",\"instruction\":\"mov        w8, w8\"},{\"address\":\"13474\",\"instruction\":\"cmp  w8, #0x0\"},{\"address\":\"13478\",\"instruction\":\"b.eq       0x1400134b0 <.text+0x24b0>\"},{\"address\":\"1347c\",\"instruction\":\"adrp     x8, 0x14001b000\"},{\"address\":\"13480\",\"instruction\":\"ldr w8, [x8, #0x274]\"},{\"address\":\"13484\",\"instruction\":\"cmp        w8, #0x0\"},{\"address\":\"13488\",\"instruction\":\"b.eq       0x1400134b0 <.text+0x24b0>\"},{\"address\":\"1348c\",\"instruction\":\"adrp     x8, 0x14001e000\"},{\"address\":\"13490\",\"instruction\":\"ldr x8, [x8, #0x30]\"},{\"address\":\"13494\",\"instruction\":\"blr x8\"},{\"address\":\"13498\",\"instruction\":\"mov      w9, w0\"},{\"address\":\"1349c\",\"instruction\":\"adrp x8, 0x14001b000\"},{\"address\":\"134a0\",\"instruction\":\"ldr w8, [x8, #0x274]\"},{\"address\":\"134a4\",\"instruction\":\"cmp        w8, w9\"},{\"address\":\"134a8\",\"instruction\":\"b.ne 0x1400134b0 <.text+0x24b0>\"}]}},{\"line_number\":22,\"hits\":22,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\",\"instruction_address\":\"1345c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13450\",\"instruction\":\"pacibsp\"},{\"address\":\"13454\",\"instruction\":\"stp        x29, x30, [sp, #-0x20]!\"},{\"address\":\"13458\",\"instruction\":\"mov x29, sp\"},{\"address\":\"1345c\",\"instruction\":\"str x0, [sp, #0x10]\"}]}},{\"line_number\":23,\"hits\":17,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\",\"instruction_address\":\"13464\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13460\",\"instruction\":\"ldr       x8, [sp, #0x10]\"},{\"address\":\"13464\",\"instruction\":\"str x8, [sp, #0x18]\"}]}},{\"line_number\":27,\"hits\":14,\"filename\":\"D:\\\\a\\\\_work\\\\1\\\\s\\\\src\\\\vctools\\\\crt\\\\vcstartup\\\\src\\\\misc\\\\debugger_jmc.c\",\"instruction_address\":\"134b4\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"134ac\",\"instruction\":\"nop\"},{\"address\":\"134b0\",\"instruction\":\"ldp      x29, x30, [sp], #0x20\"},{\"address\":\"134b4\",\"instruction\":\"autibsp\"},{\"address\":\"134b8\",\"instruction\":\"ret\"}]}}]},\r\n{\"function_name\": \"main\",\r\n\"source_code\":[{\"line_number\":62,\"hits\":15,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\main.c\",\"instruction_address\":\"13384\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"13384\",\"instruction\":\"fmov     d0, d8\"},{\"address\":\"13388\",\"instruction\":\"mov  w0, #0x1\"},{\"address\":\"1338c\",\"instruction\":\"bl 0x1400111d4 <.text+0x1d4>\"},{\"address\":\"13390\",\"instruction\":\"sub       w19, w19, #0x1\"},{\"address\":\"13394\",\"instruction\":\"cbnz w19, 0x140013384 <.text+0x2384>\"}]}},{\"line_number\":64,\"hits\":1,\"filename\":\"C:\\\\Users\\\\nader\\\\source\\\\repos\\\\windowsperfsample\\\\main.c\",\"instruction_address\":\"1339c\",\"disassembled_line\":{\"disassemble\":[{\"address\":\"1339c\",\"instruction\":\"mov x2, x20\"},{\"address\":\"133a0\",\"instruction\":\"mov x1, x21\"},{\"address\":\"133a4\",\"instruction\":\"mov x0, x22\"},{\"address\":\"133a8\",\"instruction\":\"bl  0x1400115d0 <.text+0x5d0>\"},{\"address\":\"133ac\",\"instruction\":\"sub       w19, w19, #0x1\"},{\"address\":\"133b0\",\"instruction\":\"cbnzw19, 0x14001339c <.text+0x239c>\"}]}}]}]}\r\n]}\r\n}";
#endif
        private void HandleSamplingFinished(object sender, (WperfSampling serializedOutput, string stdError) e)
        {
            SamplingSettingsMonikerButton.IsEnabled = true;
            StartSamplingMonikerButton.IsEnabled = true;
            StopSamplingMonikerButton.IsEnabled = false;
            if (!string.IsNullOrEmpty(e.stdError))
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.ErrorWindowsPerfCLI, e.stdError);
                return;
            }

            if (e.serializedOutput == null)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.NoWindowsPerfCliData, e.stdError);
                return;
            }

            formattedSamplingResults.FormatSamplingResults(e.serializedOutput,
                $"{SamplingExplorerLanguagePack.ExecutedAt} {DateTime.Now.ToShortTimeString()}");
            if (_tree.Model == null) _tree.Model = formattedSamplingResults;
            _tree.UpdateTreeList();
        }


        public SamplingExplorerControl()
        {
            InitializeComponent();
#if DEBUG
            formattedSamplingResults.FormatSamplingResults(WperfSampling.FromJson(rootSample));
#endif
            _tree.Model = formattedSamplingResults;
            wperfClient.OnSamplingFinished += HandleSamplingFinished;
            StopSamplingMonikerButton.IsEnabled = false;
        }

        private void SettingsMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WPerfOptions.Instance.IsWperfInitialized)
            {
                VS.MessageBox.ShowError(
                    ErrorLanguagePack.NotInititiatedWperfErrorLine1,
                    ErrorLanguagePack.NotInititiatedWperfErrorLine2
                );
                return;
            }

            SamplingSettingDialog samplingSettingsDialog = new();
            samplingSettingsDialog.Title = SamplingSettingsLanguagePack.WindowTitle;
            samplingSettingsDialog.ShowDialog();
        }

        private void StartSamplingMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!WPerfOptions.Instance.IsWperfInitialized) return;
            if (!SamplingSettings.AreSettingsFilled)
            {
                VS.MessageBox.ShowError(ErrorLanguagePack.UncompleteSamplingSettingsLine1,
                    ErrorLanguagePack.UncompleteSamplingSettingsLine2
                );
                return;
            }

            if (SamplingSettings.IsSampling)
            {
                VS.MessageBox.ShowError(
                    ErrorLanguagePack.RunningSamplingOverlapLine1,
                    ErrorLanguagePack.RunningSamplingOverlapLine2
                );
                return;
            }

            wperfClient.StartSamplingAsync().FireAndForget();
            SamplingSettingsMonikerButton.IsEnabled = false;
            StartSamplingMonikerButton.IsEnabled = false;
            StopSamplingMonikerButton.IsEnabled = true;
        }

        private void StopSamplingMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            wperfClient.StopSampling();
            SamplingSettingsMonikerButton.IsEnabled = true;
            StartSamplingMonikerButton.IsEnabled = true;
            StopSamplingMonikerButton.IsEnabled = false;
        }

        private void HighlightEditor(SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE)
            {
                foreach (SamplingSection child in samplingSection.Children)
                {
                    HighlightEditor(child);
                }
            }

            if (!samplingSection.IsFileExists)
            {
                return;
            }

            HighlighterDict.AddFileToHighlight(samplingSection);
        }

        private void _tree_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TreeList treeList = sender as TreeList;
            SummaryStack.Children.Clear();
            if (treeList == null || treeList.SelectedItem == null) return;
            SamplingSection selecteditem = (treeList.SelectedItem as TreeNode).Tag as SamplingSection;
            switch (selecteditem.SectionType)
            {
                case SamplingSection.SamplingSectionType.ROOT:
                    CreateRootSummary(SummaryStack.Children, selecteditem);
                    HighlighterDict.Clear();
                    HighlightEditor(selecteditem);
                    break;
                case SamplingSection.SamplingSectionType.SAMPLE_EVENT:
                    CreateEventSummary(SummaryStack.Children, selecteditem);
                    break;
                case SamplingSection.SamplingSectionType.SAMPLE_FUNCTION:
                    CreateFunctionSummary(SummaryStack.Children, selecteditem);
                    break;

                case SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE:
                    CreateSourceCodeSummary(SummaryStack.Children, selecteditem);
                    break;
                default:
                    break;
            }
        }

        private void CreateSourceCodeSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_SOURCE_CODE)
            {
                return;
            }

            children.Add(GenerateTextBlock(
                new List<Inline>()
                    { new Bold(new Run($"{SamplingExplorerLanguagePack.FilePath}: ")), new Run(samplingSection.Name) },
                _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.LineNumber}: ")),
                    new Run(samplingSection.LineNumber.ToString())
                }, _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                    { new Bold(new Run($"{SamplingExplorerLanguagePack.Hits}: ")), new Run($"{samplingSection.Hits}") },
                _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.Overhead}: ")),
                    new Run($"{samplingSection.OverheadPercentage}")
                }, _localFontSizes.md));
            StackPanel asseblyPanel = GenerateAssemblyStackPanel(samplingSection);
            if (asseblyPanel != null)
                children.Add(asseblyPanel);
        }

        const int AVERAGE_LINE_HEIGHT = 30;

        private StackPanel GenerateAssemblyStackPanel(SamplingSection samplingSection)
        {
            if (samplingSection.Assemblies == null || samplingSection.Assemblies.Count == 0)
            {
                return null;
            }

            StackPanel asseblyPanel = new StackPanel();

            Grid asseblyTitleGrid = GenerateAssemblyLineGrid("Address", "Instruction", isTitle: true);

            asseblyPanel.Children.Add(asseblyTitleGrid);

            Brush highlightColor = new SolidColorBrush(Colors.Red);
            int offset = 1;
            bool stopOffsetCount = false;
            foreach (ExtendedAssembly assemblyLine in samplingSection.Assemblies)
            {
                Grid asseblyLineGrid = GenerateAssemblyLineGrid(
                    firstColumn: assemblyLine.Address,
                    secondColumn: assemblyLine.Instruction,
                    textColor:
                    assemblyLine.IsHighlighted ? highlightColor : null
                );

                asseblyPanel.Children.Add(asseblyLineGrid);

                if (assemblyLine.IsHighlighted)
                    stopOffsetCount = true;

                if (!stopOffsetCount)
                    offset++;
            }

            int numberOfVisibleLines = (int)AssemblyScrollViewer.ActualHeight / AVERAGE_LINE_HEIGHT;
            int highlightedTextPosition = (offset + 4) * AVERAGE_LINE_HEIGHT;
            if ((offset + 5) > numberOfVisibleLines)
            {
                int centeredPositionForHighlightedText =
                    highlightedTextPosition - (numberOfVisibleLines * AVERAGE_LINE_HEIGHT / 2);
                AssemblyScrollViewer.ScrollToVerticalOffset(centeredPositionForHighlightedText);
            }
            else
            {
                AssemblyScrollViewer.ScrollToVerticalOffset(0);
            }

            return asseblyPanel;
        }

        private Grid GenerateAssemblyLineGrid(string firstColumn, string secondColumn, bool isTitle = false,
            Brush textColor = null)
        {
            Grid asseblyTitleGrid = new Grid();
            asseblyTitleGrid.ColumnDefinitions.Add(new ColumnDefinition()
                { Width = new GridLength(100, GridUnitType.Pixel) });
            asseblyTitleGrid.ColumnDefinitions.Add(new ColumnDefinition()
                { Width = new GridLength(1, GridUnitType.Star) });

            TextBlock addressTitleTB = GenerateTextBlock(
                InlineText: new List<Inline>()
                    { isTitle ? new Bold(new Run(firstColumn)) : new Run(firstColumn) },
                fontSize: isTitle ? _localFontSizes.lg : _localFontSizes.md,
                textColor: textColor
            );
            FontFamily _fontFamily = new FontFamily("Consolas");
            addressTitleTB.FontFamily = _fontFamily;
            addressTitleTB.SetValue(Grid.ColumnProperty, 0);

            asseblyTitleGrid.Children.Add(addressTitleTB);

            TextBlock instructionTitleTB = GenerateTextBlock(
                InlineText: new List<Inline>() { new Bold(new Run(secondColumn)) },
                fontSize: isTitle ? _localFontSizes.lg : _localFontSizes.md,
                textColor: textColor
            );
            instructionTitleTB.SetValue(Grid.ColumnProperty, 1);
            instructionTitleTB.FontFamily = _fontFamily;

            asseblyTitleGrid.Children.Add(instructionTitleTB);
            return asseblyTitleGrid;
        }

        private void CreateFunctionSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_FUNCTION)
            {
                return;
            }

            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.FunctionName}: ")), new Run(samplingSection.Name)
                }, _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.Interval}: ")),
                    new Run(samplingSection.Parent.Frequency)
                }, _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                    { new Bold(new Run($"{SamplingExplorerLanguagePack.Hits}: ")), new Run($"{samplingSection.Hits}") },
                _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.Overhead}: ")),
                    new Run($"{samplingSection.OverheadPercentage}")
                }, _localFontSizes.md));
        }

        private void CreateEventSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.SAMPLE_EVENT)
            {
                return;
            }

            children.Add(GenerateTextBlock(
                new List<Inline>()
                    { new Bold(new Run($"{SamplingExplorerLanguagePack.EventName}: ")), new Run(samplingSection.Name) },
                _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.Interval}: ")), new Run(samplingSection.Frequency)
                }, _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.NumberOfCollectedSamples}: ")),
                    new Run($"{samplingSection.Hits}")
                }, _localFontSizes.md));
        }

        private void CreateRootSummary(UIElementCollection children, SamplingSection samplingSection)
        {
            if (samplingSection.SectionType != SamplingSection.SamplingSectionType.ROOT)
            {
                return;
            }

            children.Add(GenerateTextBlock(
                new List<Inline>()
                    { new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")), new Run(samplingSection.Name) },
                _localFontSizes.lg, _localFontWeights.bold));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                    { new Bold(new Run($"{SamplingExplorerLanguagePack.PeFile}: ")), new Run(samplingSection.PeFile) },
                _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.PdbFile}: ")), new Run(samplingSection.PdbFile)
                }, _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.SamplesGenerated}: ")),
                    new Run($"{samplingSection.Hits}")
                }, _localFontSizes.md));
            children.Add(GenerateTextBlock(
                new List<Inline>()
                {
                    new Bold(new Run($"{SamplingExplorerLanguagePack.SamplesDropped}: ")),
                    new Run($"{samplingSection.SamplesDropped}")
                }, _localFontSizes.md));
            children.Add(GenerateTextBlock(new List<Inline>() { new Run($"{SamplingExplorerLanguagePack.Modules}: ") },
                _localFontSizes.md, _localFontWeights.bold));

            foreach (var module in samplingSection.Modules)
            {
                children.Add(GenerateTextBlock(
                    new List<Inline>()
                        { new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")), new Run(module.Name) },
                    layer: 1));
                children.Add(GenerateTextBlock(
                    new List<Inline>()
                        { new Bold(new Run($"{SamplingExplorerLanguagePack.FilePath}: ")), new Run(module.Path) },
                    layer: 2));
            }

            children.Add(GenerateTextBlock(
                new List<Inline>() { new Run($"{SamplingExplorerLanguagePack.EventList}: ") }, _localFontSizes.md,
                _localFontWeights.bold));

            foreach (var sampledEvent in samplingSection.Children)
            {
                children.Add(GenerateTextBlock(
                    new List<Inline>()
                        { new Bold(new Run($"{SamplingExplorerLanguagePack.Name}: ")), new Run(sampledEvent.Name) },
                    layer: 1));
                children.Add(GenerateTextBlock(
                    new List<Inline>()
                    {
                        new Bold(new Run($"{SamplingSettingsLanguagePack.Frequency} ")), new Run(sampledEvent.Frequency)
                    }, layer: 2));
            }
        }

        private enum _localFontSizes
        {
            sm = 12,
            md = 14,
            lg = 16,
        }

        private enum _localFontWeights
        {
            normal,
            bold,
        }

        private TextBlock GenerateTextBlock(List<Inline> InlineText, _localFontSizes fontSize = _localFontSizes.sm,
            _localFontWeights fontWeight = _localFontWeights.normal, int layer = 0, Brush textColor = null)
        {
            FontWeight _fontWeight = fontWeight == _localFontWeights.normal ? FontWeights.Normal : FontWeights.Bold;
            double marginTop = fontSize == _localFontSizes.sm ? 5 : fontSize == _localFontSizes.md ? 10 : 15;
            Thickness _margin = new Thickness(layer * 15, marginTop, 0, 0);
            TextBlock textBlock = new TextBlock()
                { FontSize = (double)fontSize, FontWeight = _fontWeight, Margin = _margin };
            textBlock.Inlines.AddRange(InlineText);
            if (textColor != null)
            {
                textBlock.Foreground = textColor;
            }

            return textBlock;
        }

        private void _tree_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeList = sender as TreeList;
            if (treeList.SelectedItem == null) return;
            SamplingSection selecteditem = (treeList.SelectedItem as TreeNode).Tag as SamplingSection;
            if (!selecteditem.IsFileExists) return;
            string filePath = selecteditem.Name;
            OpenVSDocument(filePath, (int)selecteditem.LineNumber);
        }

        private void OpenVSDocument(string filePath, int lineNumber)
        {
            _ = Task.Run(async () =>
            {
                DocumentView docView;
                docView = await VS.Documents.OpenAsync(filePath);
                SnapshotPoint position = docView.TextView.Caret.Position.BufferPosition;
                docView.TextView.Caret.MoveTo(position.Snapshot.GetLineFromLineNumber(lineNumber - 1).End);
            });
        }

        private void ClearListMonikerButton_Click(object sender, RoutedEventArgs e)
        {
            formattedSamplingResults.ClearSampling();
            _tree.UpdateTreeList();
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            string filePath = textBlock.Text;
            SamplingSection selecteditem = (_tree?.SelectedItem as TreeNode)?.Tag as SamplingSection;
            bool isFileExists = File.Exists(filePath);
            if (!isFileExists) return;
            int lineNumber = selecteditem?.LineNumber != null ? (int)selecteditem?.LineNumber : 0;
            OpenVSDocument(filePath, lineNumber);
        }
    }
}