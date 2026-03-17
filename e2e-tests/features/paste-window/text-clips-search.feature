@skip
Feature: Text Clips Search
  As a user
  I want to search through clipboard text items
  So that I can quickly find and retrieve specific content from my clipboard history

  Scenario: Background
    Given the following text clipboard items:
      | search label  |
      | text clip 1   |
      | text clip 12  |
      | text clip 123 |
    Given the Paste window is activated
