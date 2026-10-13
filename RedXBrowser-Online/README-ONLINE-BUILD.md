# RedXBrowser — online compilation

This project includes a GitHub Actions workflow that compiles the app on a hosted Windows runner. No compiler needs to be installed on your PC.

## Steps

1. Sign in to GitHub at https://github.com/ and create a **new repository** named `RedXBrowser`. For the easiest setup, make it **Private**.
2. On the new repository page, choose **Add file → Upload files**.
3. Extract this ZIP on your PC, then upload the files and folders inside the `RedXBrowser-Online` folder. Make sure `.github/workflows/build-windows.yml` is included. GitHub's web uploader may hide dot-prefixed folders; if you can't upload `.github`, use GitHub's web editor to create `.github/workflows/build-windows.yml` and paste in the workflow content from this project.
4. Commit the uploaded files to the `main` branch.
5. Open the repository's **Actions** tab. Choose **Build RedXBrowser for Windows** and click **Run workflow** if it has not already started from the commit.
6. Open the completed workflow run. Under **Artifacts**, click `RedXBrowser-Windows-x64` to download a ZIP containing `RedXBrowser.exe`.
7. Extract the downloaded ZIP and run `RedXBrowser.exe` on Windows.

## Notes

- This creates a Windows x64 desktop application; it does not run inside the browser.
- The target PC needs the Microsoft Edge WebView2 Runtime. Install it from Microsoft's official page if missing: https://developer.microsoft.com/microsoft-edge/webview2/
- GitHub Actions usage limits and availability depend on your GitHub plan and repository settings.
- The workflow builds source code in your repository. Don't upload secrets or private credentials.
