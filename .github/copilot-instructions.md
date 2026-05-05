# Copilot Instructions

## CSS / SCSS

- **Never edit `.css` or `.min.css` files directly.** Always make changes to the corresponding `.scss` source file instead.
- After modifying any `.scss` file, recompile it using [Excubo.WebCompiler](https://github.com/nicktobey/WebCompiler):
  ```bash
  dotnet tool run webcompiler -r src/Money.Blazor.Host/wwwroot/css -c src/Money.Blazor.Host/webcompilerconfiguration.json
  ```
- This generates both the `.css` and `.min.css` outputs. Commit all three files (`.scss`, `.css`, `.min.css`) together.
