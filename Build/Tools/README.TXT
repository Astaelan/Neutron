In the future an SDKInstaller will download and unpack everything needed for a build environment.
Until the SDKInstaller exists, you must manually download and extract the tool packages into the root directory.
The SDK packages are broken down into a universal host tool package, and a specific target tool package for a given host.
Example: X86-Windows_STM32F4-Proton would be tools on windows to build an OS called Proton for STM32F4 devices.
Currently, the only Host/Target available for testing is X86-Windows_X86-Windows which uses the MSVC linker and libraries, so the target tools package is empty.


X86 Windows Host Tools
https://dl.dropboxusercontent.com/u/35394461/Neutron/Tools.X86-Windows.7z


X86 Windows Host to X86 Windows Target Tools (Using MSVC linker and libraries, so this file is empty and provided for standarization)
https://dl.dropboxusercontent.com/u/35394461/Neutron/Tools.X86-Windows_X86-Windows.7z


Note: The target tools packages may be removed if the LLVM linker is able to universally target multiple architectures other than Windows.
However, target library packages (IE: Libraries.STM32F4-Proton.7z) will still be required for the target C/C++ standard libraries