// ============================================================================
// Copyright (c) Murr (https://github.com/vtstv)
// All rights reserved.
// ============================================================================

using FolderToPDF.Core.Services;
using Xunit;

namespace FolderToPDF.Tests;

public class ContentCleanerServiceTests
{
    private readonly ContentCleanerService _cleaner = new();

    [Fact]
    public void RemoveComments_CSharp_RemovesSingleAndMultiLineComments()
    {
        var code = """
            // This is a single line comment
            public class Foo
            {
                /* This is a 
                   multi-line comment */
                public int Bar => 42; // Inline comment
            }
            """;

        var cleaned = _cleaner.RemoveComments(code, ".cs");

        Assert.DoesNotContain("//", cleaned);
        Assert.DoesNotContain("/*", cleaned);
        Assert.DoesNotContain("multi-line comment", cleaned);
        Assert.Contains("public class Foo", cleaned);
        Assert.Contains("public int Bar => 42;", cleaned);
    }

    [Fact]
    public void RemoveComments_Python_RemovesHashComments()
    {
        var code = """
            # This is python comment
            def hello():
                print("world") # inline comment
            """;

        var cleaned = _cleaner.RemoveComments(code, ".py");

        Assert.DoesNotContain("#", cleaned);
        Assert.Contains("def hello():", cleaned);
        Assert.Contains("print(\"world\")", cleaned);
    }

    [Fact]
    public void RemoveComments_HtmlAndXml_RemovesXmlComments()
    {
        var markup = """
            <!-- Header section -->
            <Grid>
                <!-- Child item -->
                <TextBlock Text="Hello" />
            </Grid>
            """;

        var cleaned = _cleaner.RemoveComments(markup, ".xaml");

        Assert.DoesNotContain("<!--", cleaned);
        Assert.DoesNotContain("-->", cleaned);
        Assert.Contains("<TextBlock Text=\"Hello\" />", cleaned);
    }

    [Fact]
    public void RedactSensitiveData_RedactsOpenAiAndAnthropicKeys()
    {
        var input = """
            OPENAI_KEY = "sk-proj-abc123456789012345678901234567890"
            ANTHROPIC_KEY = "sk-ant-api03-abcdefghijklmnopqrstuvwxyz1234"
            """;

        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("abc123456789012345678901234567890", redacted);
        Assert.DoesNotContain("api03-abcdefghijklmnopqrstuvwxyz1234", redacted);
        Assert.Contains("[REDACTED_OPENAI_KEY]", redacted);
        Assert.Contains("[REDACTED_ANTHROPIC_KEY]", redacted);
    }

    [Fact]
    public void RedactSensitiveData_RedactsGithubTokensAndAwsKeys()
    {
        var input = """
            GH_TOKEN=ghp_123456789012345678901234567890123456
            AWS_ACCESS=AKIAIOSFODNN7EXAMPLE
            """;

        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("123456789012345678901234567890123456", redacted);
        Assert.DoesNotContain("AKIAIOSFODNN7EXAMPLE", redacted);
        Assert.Contains("[REDACTED_GITHUB_TOKEN]", redacted);
        Assert.Contains("[REDACTED_AWS_KEY]", redacted);
    }

    [Fact]
    public void RedactSensitiveData_RedactsPasswordAssignments()
    {
        var input = """
            password = "SuperSecretPassword123!"
            api_key: 'my-custom-api-token'
            """;

        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("SuperSecretPassword123!", redacted);
        Assert.DoesNotContain("my-custom-api-token", redacted);
        Assert.Contains("<REDACTED_SECRET>", redacted);
    }

    [Fact]
    public void RedactSensitiveData_RedactsEmailsAndPreservesLocalhost()
    {
        var input = """
            Contact user@example.com for support.
            Server listening at 127.0.0.1:8080 and 0.0.0.0:443.
            Connecting to 198.51.100.42 now.
            """;

        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("user@example.com", redacted);
        Assert.Contains("<REDACTED_EMAIL>", redacted);
        Assert.Contains("127.0.0.1", redacted);
        Assert.Contains("0.0.0.0", redacted);
        Assert.DoesNotContain("198.51.100.42", redacted);
        Assert.Contains("<REDACTED_IP>", redacted);
    }

    [Fact]
    public void Clean_HandlesEmptyOrNullInputGracefully()
    {
        Assert.Equal(string.Empty, _cleaner.Clean(string.Empty, ".cs", true, true));
        Assert.Equal(string.Empty, _cleaner.Clean("   ", ".cs", true, true));
        Assert.Equal(string.Empty, _cleaner.Clean(null!, ".cs", true, true));
    }

    [Fact]
    public void RedactSensitiveData_RedactsJwtAndBearerTokens()
    {
        var input = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.doNotLeakThisSecretSignature123456";
        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("doNotLeakThisSecretSignature123456", redacted);
        Assert.Contains("[REDACTED_BEARER_TOKEN]", redacted);
    }

    [Fact]
    public void RedactSensitiveData_RedactsStandaloneJwtToken()
    {
        var input = "token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.doNotLeakThisSecretSignature123456";
        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("doNotLeakThisSecretSignature123456", redacted);
        Assert.Contains("eyJ[REDACTED_JWT_TOKEN]", redacted);
    }

    [Fact]
    public void RedactSensitiveData_RedactsRsaPrivateKeyBlock()
    {
        var input = """
            -----BEGIN RSA PRIVATE KEY-----
            MIIEowIBAAKCAQEA0Y1234567890abcdefghijklmnopqrstuvwxyzABCDEF==
            -----END RSA PRIVATE KEY-----
            """;
        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.DoesNotContain("MIIEowIBAAKCAQEA0Y1234567890", redacted);
        Assert.Contains("[REDACTED_PRIVATE_KEY]", redacted);
    }

    [Fact]
    public void RemoveComments_UnknownExtension_ReturnsContentUnchanged()
    {
        var input = "some random plain text with # and // marks";
        var result = _cleaner.RemoveComments(input, ".unknownext");

        Assert.Equal(input, result);
    }

    [Fact]
    public void RemoveComments_CompressesMultipleBlankLines()
    {
        var input = "Line 1\n\n\n\n\nLine 2";
        var result = _cleaner.RemoveComments(input, ".cs");

        Assert.Equal("Line 1\n\nLine 2", result);
    }

    [Fact]
    public void RemoveComments_CSharp_PreservesUrlsInStrings()
    {
        var input = """
            string apiUrl = "https://api.github.com/v1/repos"; // fetch repos
            string wsUrl = "wss://stream.binance.com:9443/ws";
            """;
        var result = _cleaner.RemoveComments(input, ".cs");

        Assert.Contains("https://api.github.com/v1/repos", result);
        Assert.Contains("wss://stream.binance.com:9443/ws", result);
        Assert.DoesNotContain("fetch repos", result);
    }

    [Fact]
    public void RemoveComments_CSharp_PreservesBlockCommentsInStrings()
    {
        var input = """
            string pattern = "/* not a comment */";
            """;
        var result = _cleaner.RemoveComments(input, ".cs");

        Assert.Equal(input.Trim(), result);
    }

    [Fact]
    public void RemoveComments_Python_PreservesHashInStringsAndHexColors()
    {
        var input = """
            color = " #ff0000"
            endpoint = "https://example.com/api#section" # inline comment
            """;
        var result = _cleaner.RemoveComments(input, ".py");

        Assert.Contains("\" #ff0000\"", result);
        Assert.Contains("https://example.com/api#section", result);
        Assert.DoesNotContain("inline comment", result);
    }

    [Fact]
    public void RedactSensitiveData_PreservesKeyAssignmentSpacingAndDelimiters()
    {
        var input = """
            secret="mysecretpassword"
            api_key: 'custom-api-token'
            password = "another_secret_password"
            """;
        var redacted = _cleaner.RedactSensitiveData(input);

        Assert.Contains("secret=\"<REDACTED_SECRET>\"", redacted);
        Assert.Contains("api_key: '<REDACTED_SECRET>'", redacted);
        Assert.Contains("password = \"<REDACTED_SECRET>\"", redacted);
    }
}

