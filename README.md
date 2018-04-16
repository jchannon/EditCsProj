# EditCsProj

This tool allow you to add and update information inside a csproj file.

## .Net Core 2.1 Global Tool

You will need to add `<PackAsTool>true</PackAsTool>` to the csproj and have the latest 2.1 SDK installed.  

### Usage


#### Adding element to `<PropertyGroup>`

This will add `<Test>1</Test>` to the first `<PropertyGroup>` in the csproj

`editcsproj -f Foo.csproj -ap "<Test>1</Test>"`

#### Updating attribute

This will update an existing attribute. In this example we tell it to search for elements called `PackageReference`, we then filter those down by searching for attributes called `Include` whose value is `FluentValidation`, we then set the `Version` attribute to the value of `9.1.2`.

`editcsproj -f Foo.csproj updateattribute PackageReference Include FluentValidation Version 9.1.2`

#### Updating element

This will update an existing element.

`editcsproj -f Foo.csproj updateelement Version 9.1.2`

#### Add element to existing `<ItemGroup>`

This will add an element to an existing `<ItemGroup>`

`editcsproj -f Foo.csproj -ae "<Test Foo=\"Bar\" />`

#### Add element to new `<ItemGroup>`

This will add an element and create a new `<ItemGroup>` in the csproj

`editcsproj -f Foo.csproj -ai "<Test Foo=\"Bar\" />`

