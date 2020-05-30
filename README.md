# FM RESP CLI

![build](https://github.com/frozenmountain/fm-resp/workflows/build/badge.svg) ![license](https://img.shields.io/badge/License-MIT-yellow.svg) ![release](https://img.shields.io/github/v/release/frozenmountain/fm-resp.svg)

The FM RESP CLI lets you parse [RESP (REdis Serialization Protocol)](https://redis.io/topics/protocol) streams.

Requires .NET Core 3.1 or newer.

## Building

Use `dotnet publish` to create a single, self-contained file for a specific platform/architecture:

### Windows
```
dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true -o win
```

### macOS
```
dotnet publish -r osx-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true -o osx
```

### Linux
```
dotnet publish -r linux-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true -o linux
```

Alternatively, use `dotnet build` to create a platform-agnostic bundle (the .NET Core runtime must be installed):

```
dotnet build
```

Using this approach will generate a library instead of an executable.

Use `dotnet fmresp.dll` instead of `fmresp` to run it.

## Usage

```
fmresp [verb] [options]
```

### Verbs
```
  analyze    Analyzes a RESP stream.

  export     Exports a RESP stream.
```

## analyze

The `analyze` verb analyzes a RESP stream.

### Usage
```
  -i, --input       Required. The input file path.

  --input-offset    (Default: 0) The offset into the input file at which to
                    start reading.
```

### File Format
The input file should follow the [RESP (REdis Serialization Protocol)](https://redis.io/topics/protocol). The following data types are supported:

#### Simple Strings
```
+OK
```
```
+QUEUED
```
```
+PONG
```

#### Errors
```
-ERR unknown command 'foobar'
```
```
-WRONGTYPE Operation against a key holding the wrong kind of value
```

#### Integers
```
:0
```
```
:1
```
```
:1000
```

#### Bulk Strings
```
$6
foobar
```
The empty string:
```
$0

```
The `null` string:
```
$-1
```

#### Arrays
A bulk string array:
```
*2
$3
foo
$3
bar
```
An integer array:
```
*3
:1
:2
:3
```
A mixed type array:
```
*5
:1
:2
:3
:4
$6
foobar
```
The empty array:
```
*0
```
The null array:
```
*-1
```

## Contact

To learn more, visit [frozenmountain.com](https://www.frozenmountain.com).

For inquiries, contact [sales@frozenmountain.com](mailto:sales@frozenmountain.com).

All contents copyright © Frozen Mountain Software.
