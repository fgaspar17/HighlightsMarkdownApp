# HighlightsMarkdownApp

**HighlightsMarkdownApp** is a .NET 10 Uno Platform desktop application for exporting highlights from Instapaper to a local Markdown file. 

## Tech Stack

- **.NET 10**
- **Uno Platform**
- **Library for OAuth 1.0a**

## Goals

  - [x] Learn how to create a Desktop application with Uno Platform  
  - [x] Gain experience with API authentication  
  - [x] Implement OAuth 1.0a authentication and secure APIs  
  - [x] Explore Credential Manager from Windows
  - [x] Write integration and unit tests for a file-based API  

## Features

- Export Markdown

  - Export markdown files from Instapaper highligths.
  - Select which files do you want to export.
  - Options to Select All and Clear Selection.
 <img width="841" height="240" alt="image" src="https://github.com/user-attachments/assets/51885101-ce05-4a57-b75f-37d313826617" />

- Authorization & Authentication

  - OAuth 1.0a implementation.
  - Auto-Login via **Windows Credential Manager** where the token is saved.

## Challenges

- OAuth 1.0a authentication.
- API calls to Instapaper.
- Parsing JSON data.
- Writing XAML for the design.
- Managing credentials.

## Lessons Learned

- Reading and implementing a basic OAuth 1.0 library.
- Using XAML for the designer.
- MVVM architecture.
- Windows Credential Manager.
- Parsing JSON in .NET using Text.Json.

## Areas to Improve

- Learn more methods of authentication.
- Learn more about MVVM architecture.
- Explore vault for secrets.

## Resources used

- StackOverflow posts
- ChatGPT
- [Instapaper API Documentation](https://www.instapaper.com/api/full)
- [OAuth 1.0a Documentation](https://oauth.net/core/1.0a/#RFC3986)
- [Uno Platform Documentation](https://platform.uno/docs/articles/intro.html)
