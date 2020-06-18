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
  analyze    Analyzes an RESP stream.

  export     Exports an RESP stream to JSON.

  filter     Filters exported JSON.
```

## analyze

The `analyze` verb analyzes an RESP stream for integrity and gives a summary of its data types.

### Usage
```
  -i, --input       Required. The input file path.

  --input-offset    (Default: 0) The offset into the input file at which to
                    start reading.
```

## export

The `export` verb exports an RESP stream to JSON for easier reading and manipulation.

### Usage
```
  -i, --input     Required. The input file path.

  -o, --output    The output file path. If not set, output is directed to
                  stdout.

  -y              (Default: false) Overwrite the output file path, if present.

  --indented      (Default: false) Use indented output formatting.
```

## filter

The `filter` verb filters exported JSON to assist with analysis of large exported files.

### Usage
```
  -i, --input                   Required. The input file path.

  -o, --output                  The output file path. If not set, output is
                                directed to the stdout.

  -y                            (Default: false) Overwrite the output file path,
                                if present.

  --indented                    (Default: false) Use indented output formatting.

  --no-simple-strings           (Default: false) Filter top-level simple
                                strings.

  --no-errors                   (Default: false) Filter top-level errors.

  --no-integers                 (Default: false) Filter top-level integers.

  --no-bulk-strings             (Default: false) Filter top-level bulk strings.

  --no-arrays                   (Default: false) Filter top-level arrays.

  --no-simple-string-values     (Default: false) Filter top-level simple string
                                values.

  --no-error-values             (Default: false) Filter top-level error values.

  --no-integer-values           (Default: false) Filter top-level integer
                                values.

  --no-bulk-string-values       (Default: false) Filter top-level bulk string
                                values.

  --no-array-values             (Default: false) Filter top-level array values.

  --min-simple-string-length    Minimum top-level simple string length
                                (inclusive).

  --min-error-length            Minimum top-level error length (inclusive).

  --min-integer                 Minimum top-level integer (inclusive).

  --min-integer-length          Minimum top-level integer length (inclusive).

  --min-bulk-string-length      Minimum top-level bulk string length
                                (inclusive).

  --min-array-length            Minimum top-level array length (inclusive).

  --max-simple-string-length    Maximum top-level simple string length
                                (inclusive).

  --max-error-length            Maximum top-level error length (inclusive).

  --max-integer                 Maximum top-level integer (inclusive).

  --max-integer-length          Maximum top-level integer length (inclusive).

  --max-bulk-string-length      Maximum top-level bulk string length
                                (inclusive).

  --max-array-length            Maximum top-level array length (inclusive).

  --from-index                  The index of the first top-level element to
                                include.

  --to-index                    The index of the last top-level element to
                                include.
```

## RESP Format
RESP streams must follow the [RESP (REdis Serialization Protocol)](https://redis.io/topics/protocol).

The following data types are supported:

### Simple Strings
```
+OK
```
```
+QUEUED
```
```
+PONG
```

### Errors
```
-ERR unknown command 'foobar'
```
```
-WRONGTYPE Operation against a key holding the wrong kind of value
```

### Integers
```
:0
```
```
:1
```
```
:1000
```

### Bulk Strings
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

### Arrays
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
