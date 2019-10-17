################################################################################
# Makefile for building and cleaning the solution
################################################################################

# explicitly set shell (required for Windows)
SHELL := C:\Windows\System32\cmd.exe

# default values, overwritten for Release
# NOTE: Variables using these values should be set using '=' instead of ':=' 
# 	to allow for recursive expansion if release rule changes default
#	debug values to release values
CONFIG := Debug
VERSION := 0.0
VERSION_FULL = $(VERSION).*.*

# Tools and related variables
TDD_TOOL := .\packages\NUnit.ConsoleRunner.3.10.0\tools\nunit3-console.exe
TDD_DLL = .\UnitTests\bin\$(CONFIG)\UnitTests.dll
TDD_DIR := .\OpenCover

COVERAGE_TOOL := .\packages\OpenCover.4.7.922\tools\OpenCover.Console.exe 
COVERAGE_REPORT_TOOL := .\packages\ReportGenerator.4.3.1\tools\net47\ReportGenerator.exe
COVERAGE_REPORT := $(TDD_DIR)\results.xml

DOXYGEN := .\packages\Doxygen.1.8.14\tools\doxygen.exe
DOXYGEN_CONFIG := .\doxygenConfiguration.txt
DOXYGEN_DIR := .\Doxygen\html

# Git and version information
GIT_LONG_HASH := $(shell git rev-parse HEAD)
GIT_SHORT_HASH := $(shell git rev-parse --short HEAD)

SHARED_ASSEMBLY_FILE := .\SharedAssemblyInfo.cs
VERSION_FILE := .\version.txt

# Solution information
SOLUTION := Lox
SOLUTION_FILE := $(SOLUTION).sln


# Zips a the specified files and move it to the specified location
# $1 Files to be zipped
# $2 Name of the zip file
# $3 Location to place the zip file
define zip_files
	@echo _
	@echo -----------------------------------
	@if not ["$1"]==[] (7z.exe a -tzip -mx9 $2 $1 && @move $2 $3) ELSE ( @echo Error: No files && exit 1 )
endef

# Runs MSbuild
# $1 Build target (IE: Clean, Build, Rebuild)
# $2 Build configuration (IE: Debug, Release)
define build_solution
	msbuild.exe $(SOLUTION_FILE) /v:q /nologo /t:$1 /p:Configuration=$2
endef

# Files that are copied to the artifacts folder for builds.
# Note: starting with ".\" copies just the file and not the path to the artifacts folder.
PACKAGE_CONTENTS = \
	.\Lox\bin\$(CONFIG)\* \
	
PACKAGE_CONTENTS_COVERAGE := \
	.\$(TDD_DIR)\* \
	
PACKAGE_CONTENTS_DOXYGEN := \
	$(DOXYGEN_DIR)\* \

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
	$(call build_solution,Rebuild,$(CONFIG))

# his rule runs TDD
.PHONY: tdd
tdd:
	@echo _
	@echo -----------------------------------
	@echo Reseting tdd directory ...
	@echo -----------------------------------
	@if EXIST $(TDD_DIR) rmdir $(TDD_DIR) /q /s;
	@mkdir $(TDD_DIR)
	@echo _
	@echo -----------------------------------
	@echo Running tests w/ coverage ...
	@echo -----------------------------------
	$(COVERAGE_TOOL) -target:$(TDD_TOOL) -targetargs:"$(TDD_DLL) --work=$(TDD_DIR)" -register:user -output:$(COVERAGE_REPORT)
	@echo _
	@echo -----------------------------------
	@echo Converting coverage report to HTML ...
	@echo -----------------------------------
	$(COVERAGE_REPORT_TOOL) -reports:$(COVERAGE_REPORT) -targetdir:$(TDD_DIR) -assemblyFilters:-nunit.framework -verbosity:Warning -tag:$(GIT_LONG_HASH)

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
	@echo [assembly: AssemblyVersion("$(VERSION_FULL)")] >> $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyFileVersion("$(VERSION_FULL)")] >> $(SHARED_ASSEMBLY_FILE)

# This rule clears the contents of the SharedAssemblyInfo.cs file. By doing this
# we ensure that if someone builds locally using visual studio, the version numbers
# and git commit will always be 0.0.*.* and empty, respectively.
.PHONY: clear_assembly_info
clear_assembly_info:
	@echo _
	@echo -----------------------------------
	@echo Erasing version and git hash to
	@echo prevent rogue local builds ...
	@echo -----------------------------------
	@echo using System.Reflection; > $(SHARED_ASSEMBLY_FILE)
	@echo [assembly: AssemblyInformationalVersion("")] >> $(SHARED_ASSEMBLY_FILE)
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

# This rule overwrites default debug variable values with release values
.PHONY: release_config
release_config:
	$(eval CONFIG := Release)
	$(eval VERSION := $(shell type $(VERSION_FILE)))

# This rule creates release package
.PHONY: release
release: release_config package

# This rule creates debug package 
.PHONY: debug
debug: package

# This rule builds and tags a release package in git
# This rule should only be run once for a final release build
.PHONY: tag
tag: release package
	@echo _
	@echo -----------------------------------
	@echo Tagging release ...
	@echo -----------------------------------
	$(shell git tag v$(VERSION))

# This rule cleans the project (removes binaries, etc).
.PHONY: clean
clean:
	@echo _
	@echo -----------------------------------
	@echo Cleaning solution and artifacts ...
	@echo -----------------------------------
	@if EXIST artifacts rmdir artifacts /s /q;
	@if EXIST $(TDD_DIR) rmdir $(TDD_DIR) /s /q;
	@if EXIST $(DOXYGEN_DIR) rmdir $(DOXYGEN_DIR) /s /q;
	$(call build_solution,Clean,Release)
	$(call build_solution,Clean,Debug)	
