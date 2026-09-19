#!/usr/bin/env python3
from __future__ import annotations

import tempfile
import unittest
from pathlib import Path

import validate_csharp_structure


class CSharpStructureValidatorTests(unittest.TestCase):
    def setUp(self) -> None:
        self.root = Path(tempfile.mkdtemp(prefix="csharp-structure-"))

    def write(self, content: str) -> Path:
        path = self.root / "Fixture.cs"
        path.write_text(content, encoding="utf-8")
        return path

    def test_balanced_source(self) -> None:
        path = self.write(
            """
namespace Demo;

public sealed class Example
{
    public void Run()
    {
        var text = "quoted { brace }";
        if (text.Length > 0)
        {
            _ = text;
        }
    }
}
"""
        )

        self.assertEqual(
            validate_csharp_structure.validate_file(path, self.root),
            [],
        )

    def test_verbatim_string_and_comments_are_ignored(self) -> None:
        path = self.write(
            r'''
namespace Demo;

public sealed class Example
{
    private const string Text = @"{ /* not a comment */ }";
    // } ] )
    public void Run()
    {
        /* { [ ( */
        var value = 1;
        _ = value;
    }
}
'''
        )

        self.assertEqual(
            validate_csharp_structure.validate_file(path, self.root),
            [],
        )

    def test_unbalanced_brace_is_reported(self) -> None:
        path = self.write(
            """
namespace Demo;

public sealed class Example
{
    public void Run()
    {
        if (true)
        {
            _ = 1;
        }
}
"""
        )

        errors = validate_csharp_structure.validate_file(path, self.root)
        self.assertTrue(any("unbalanced delimiters" in error for error in errors))

    def test_class_scope_executable_statement_is_reported(self) -> None:
        path = self.write(
            """
namespace Demo;

public sealed class Example
{
    public bool Exists(string id)
    {
        return id.Length > 0;
    }

    ArgumentException.ThrowIfNullOrWhiteSpace(id);
}
"""
        )

        errors = validate_csharp_structure.validate_file(path, self.root)
        self.assertTrue(
            any("class scope" in error for error in errors),
            errors,
        )

    def test_negative_delimiter_depth_is_reported(self) -> None:
        path = self.write(
            """
namespace Demo;

public sealed class Example
{
}
)
"""
        )

        errors = validate_csharp_structure.validate_file(path, self.root)
        self.assertTrue(
            any("negative parenthesis depth" in error for error in errors),
            errors,
        )


if __name__ == "__main__":
    unittest.main()
