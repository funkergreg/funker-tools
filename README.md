# funker-tools

This repository contains tools to work with sprite assets in the Unity Editor.

## Purpose

The purpose of `funker-tools` is to work with assets with as little movement or copying as possible of the original assets and as few custom directories as possible.  Most tools will be usable as options in the Unity editor, usually with specific assets or folders selected.  The tools are designed as modular steps to support chaining utilities together to do more complex (or repetitive) work.

## Usage

1. Add `funker-tools` to the Unity project
   1. This repository is designed to be its own directory (i.e. clone under your Unity project's `Assets` folder).
1. In the Unity editor, select the sprite sheet(s) in the `Project` panel
   1. To select multiple, shift-click or crtl+click sprite sheets; not designed to recurse over directories
1. Click the drop-down `Tools` menu item
1. Select the desired tool

![UsageToolsMenu](/docs/tools-scripts2.png)

### Tools

- SpriteSlicer_16x16
  - Slices each selected sprite sheet into 16x16 squares
- SpriteSlicer_32x32
  - Slices each selected sprite sheet into 32x32 squares
- SpriteSlicer_64x64
  - Slices each selected sprite sheet into 64x64 squares
- SpriteSlicer_Regex
  - Attempts to slice selected sprite sheets at `width`x`height` as designated in the filename; if the expected REGEX is not present a popup window prompts for width and height input

#### Notes

By default, the sprite slicers currently apply:

- Standard pixels per unit:
  - `spritePixelsPerUnit = 64`
- The anchor at the bottom, centered.
  - `pivot = new Vector2(0.5f, 0f)`
  - `alignment = SpriteAlignment.BottomCenter`

If the sprite sheet is not evenly sliceable by the width and height given, script behavior is indeterminate.

## Licensing

WARNING: This project is a work-in-progress for experiemntal hobbyist development.

All code in this repository is provided as open source resources as-is, with no guarantee of functionality, interoperability, or future updates under the Apache-2.0 license.

## Unity

- The Unity engine: <https://unity.com/products/unity-engine>
