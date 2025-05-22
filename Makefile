build:
	mkdir -p artifacts
	dotnet publish TestApp/TestApp.csproj -r osx-x64 -c Release
	dotnet publish TestApp/TestApp.csproj -r osx-arm64 -c Release
	lipo -create -output artifacts/TestApp TestApp/bin/Release/net9.0/osx-x64/publish/TestApp TestApp/bin/Release/net9.0/osx-arm64/publish/TestApp
	# Dylibs should have both architectures
	cp TestApp/bin/Release/net9.0/osx-x64/publish/*.dylib artifacts/

clean:
	git clean -f -d -x