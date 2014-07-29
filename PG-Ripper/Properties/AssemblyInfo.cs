// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="The Watcher">
//   Copyright (c) The Watcher Partial Rights Reserved.
//  This software is licensed under the MIT license. See license.txt for details.
// </copyright>
// <summary>
//   Code Named: PG-Ripper
//   Function  : Extracts Images posted on RiP forums and attempts to fetch them to disk.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

#if (PGRIPPERX)

[assembly: AssemblyTitle("Forums Image Ripper X")]
[assembly: AssemblyProduct("PG-Ripper X")]

#else

[assembly: AssemblyTitle("Forums Image Ripper")]
[assembly: AssemblyProduct("PG-Ripper")]

#endif

[assembly: AssemblyDescription("Rips Images from VB Forums")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Ripper.CodePlex.com")]

[assembly: AssemblyCopyright("(c) The Watcher")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("1.4.3.2")]
[assembly: AssemblyFileVersionAttribute("1.4.3.2")]

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyName("")]