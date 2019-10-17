################################################################################
# Makefile for building and cleaning the solution
################################################################################
SHELL=C:\Windows\System32\cmd.exe
CONFIG:=Debug
TDD_TOOL:=.\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe
TDD_DLL:=.\UnitTests\bin\$(CONFIG)\UnitTests.dll
COVERAGE_TOOL:=.\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe 
COVERAGE_REPORT_TOOL:=.\packages\ReportGenerator.4.3.1\tools\net47\ReportGenerator.exe
COVERAGE_DIR:=.\OpenCover
COVERAGE_REPORT:=$(COVERAGE_DIR)\results.xml
DOXYGEN:=.\packages\Doxygen.1.8.14\tools\doxygen.exe
DOXYGEN_CONFIG:=.\doxygenConfiguration.txt
GIT_LONG_HASH:=$(shell git rev-parse HEAD)
GIT_SHORT_HASH:=$(shell git rev-parse --short HEAD)
SHARED_ASSEMBLY_FILE:=.\SharedAssemblyInfo.cs
VERSION_FILE:=.\version.txt
SOLUTION:=Lox
SOLUTION_FILE:=$(SOLUTION).sln

# Debug default value
VERSION:=0.0
# Use version file value for release
ifeq ($(CONFIG),Release)
	VERSION:=$(shell type $(VERSION_FILE))
endif

# Zips a the specified files and move it to the specified location
# $1 Files to be zipped
# $2 Name of the zip file
# $3 Location to place the zip file
define zip_files
	@echo _
	@echo -----------------------------------
	@if not ["$1"]==[] (7z.exe a -tzip -mx9 $2 $1 && @move $2 $3) ELSE ( @echo Error: No files && exit 1 )
endef

# define build_solution
	# msbuild.exe $(SOLUTION_FILE) /v:q /nologo /t:$1 /p:Configuration=$2
# endef

# Files that are copied to the artifacts folder for builds.
# Note: starting with ".\" copies just the file and not the path to the artifacts folder.
PACKAGE_CONTENTS:=\
	.\Lox\bin\$(CONFIG)\* \
	
PACKAGE_CONTENTS_COVERAGE:=\
	.\$(COVERAGE_DIR)\* \
	
PACKAGE_CONTENTS_DOXYGEN:=\
	.\Doxygen\html\* \

# Default rule.
.PHONY: all
all: tdd

# This rule restores any pertinent nuget packages for the solution.
.PHONY: nuget
nuget:
	@echo _
	@echo -----------------------------------
	@echo Restoring all NuGet packages ...
	@echo -----------------------------------
	nuget.exe restore $(SOLUTION_FILE)

# This rule builds the solution using build configuration
.PHONY: build
build: nuget
	@echo _
	@echo -----------------------------------
	@echo Building Solution ($(CONFIG)) ...
	@echo -----------------------------------
	msbuild.exe $(SOLUTION_FILE) /v:q /nologo /t:Rebuild /p:Configuration=$(CONFIG)

# This rule resets the coverage directory
.PHONY: reset_coverage_dir
reset_coverage_dir:
	@echo _
	@echo -----------------------------------
	@echo Reseting coverage directory ...
	@echo -----------------------------------
	@if exist $(COVERAGE_DIR) (rd $(COVERAGE_DIR) /q /s)
	@md $(COVERAGE_DIR)
	
# This rule runs nunit and coverage
.PHONY: nunit
nunit: build reset_coverage_dir
	@echo _
	@echo -----------------------------------
	@echo Running TDD tests w/ coverage ...
	@echo -----------------------------------
	$(COVERAGE_TOOL) -target:$(TDD_TOOL) -targetargs:"$(TDD_DLL) --work=$(COVERAGE_DIR)" -register:user -output:$(COVERAGE_REPORT)

# This rule converts OpenCover XML report to HTML
.PHONY: coverage_report
coverage_report: nunit 
	@echo _
	@echo -----------------------------------
	@echo Converting coverage report to HTML ...
	@echo -----------------------------------
	$(COVERAGE_REPORT_TOOL) -reports:$(COVERAGE_REPORT) -targetdir:$(COVERAGE_DIR) -assemblyFilters:-nunit.framework -verbosity:Warning -tag:$(GIT_LONG_HASH)

# This rule runs TDD
.PHONY: tdd
tdd: coverage_report

# This rule generates a shared assembly info file that sets the git hash and version number
# for all build assemblies (DLLs and EXEs) in the solution.
.PHONY: set_assembly_info
set_assembly_info:
	@echo _
	@echo -----------------------------------
	@echo Injecting version and git hash:
	@echo Version: $(VERSION) ($(CONFIG))
	@echo Githash: $(GIT_LONG_HASH)
	@echo -----------------------------------
	@echo using System.Reflection; > $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyInformationalVersion("$(CONFIG):$(GIT_LONG_HASH)")] >> $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyVersion("$(VERSION).*.*")] >> $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyFileVersion("$(VERSION).*.*")] >> $(SHARED_ASSEMBLY_FILE)

# This rule clears the contents of the SharedAssemblyInfo.cs file. By doing this
# we ensure that if someone builds locally using visual studio, the version numbers
# and git commit will always be 0.0.0.0 and empty, respectively.
.PHONY: clear_assembly_info
clear_assembly_info:
	@echo _
	@echo -----------------------------------
	@echo Erasing version and git hash to
	@echo prevent rogue local builds ...
	@echo -----------------------------------
	@echo using System.Reflection; > $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyInformationalVersion("$(CONFIG)")] >> $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyVersion("0.0.*.*")] >> $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyFileVersion("0.0.*.*")] >> $(SHARED_ASSEMBLY_FILE)

# This rule generates Doxygen documentation
.PHONY: doxygen
doxygen:
	@echo _
	@echo -----------------------------------
	@echo Generating doxygen docs ...
	@echo -----------------------------------
	$(DOXYGEN) $(DOXYGEN_CONFIG)

# This rule packages a build.
.PHONY: package
package: set_assembly_info tdd doxygen clear_assembly_info
	@echo _
	@echo -----------------------------------
	@echo Zipping up artifacts ...
	@echo -----------------------------------
	@if NOT EXIST artifacts mkdir artifacts;
	$(call zip_files,$(PACKAGE_CONTENTS),$(SOLUTION)_$(CONFIG)_$(VERSION)_$(GIT_SHORT_HASH).zip,artifacts)
	$(call zip_files,$(PACKAGE_CONTENTS_DOXYGEN),$(SOLUTION)_$(CONFIG)_Documentation_$(VERSION)_$(GIT_SHORT_HASH).zip,artifacts)
	$(call zip_files,$(PACKAGE_CONTENTS_COVERAGE),$(SOLUTION)_$(CONFIG)_TDD_$(VERSION)_$(GIT_SHORT_HASH).zip,artifacts)

# This rule tags a package in git
.PHONY: tag
tag: package
	@echo 
	@echo -----------------------------------
	@echo Tagging release ...
	@echo -----------------------------------
	$(shell git tag v$(VERSION))

# This rule cleans the project (removes binaries, etc).
.PHONY: clean
clean:
	@echo 
	@echo -----------------------------------
	@echo Cleaning solution and artifacts ...
	@echo -----------------------------------
	@if EXIST artifacts rmdir artifacts /s /q;
	msbuild.exe $(SOLUTION_FILE) /v:q /nologo /t:Clean /p:Configuration=Release
	msbuild.exe $(SOLUTION_FILE) /v:q /nologo /t:Clean /p:Configuration=Debug
	
