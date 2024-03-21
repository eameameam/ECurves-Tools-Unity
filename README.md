# ECurves Tools - Animation Clip Tools.

## ECurveRenamer
`ECurveRenamer` is a Unity Editor extension designed to streamline the animation workflow. This powerful tool simplifies the process of renaming animation curve paths within animation clips, a task that can be tedious when done manually. It's especially useful for technical animators and anyone dealing with complex animation hierarchies.

![ECurveRenamer Window](/ECurveRenamer.png)

## Installation

1. Copy `ECurveRenamer.cs` into your Unity project's Editor folder.

## Usage

Access the editor window through Unity's menu by going to Escripts > ERenamer Animation Curves.

## Curve Path Renaming
Rename curve paths across multiple selected animation clips with ease.
Perfect for batch-renaming operations after rigging changes or hierarchy updates in your animations.

## Quick Renaming
1. Select the animation clips you want to modify in the Project window.
2. Input the old name and the new name you wish to apply.
3. Use the toggles to specify if you want to rename curves for position, rotation, and scale, including their respective axes.
4. Click "Rename Curves" to apply the changes.

## Asset Database Management
Choose whether to immediately save changes to the Asset Database.


## ECurves Remover
`ECurvesRemover` is another essential Unity Editor extension for managing animation curves. It enables users to delete unwanted curves from animation clips or modify their hierarchy paths, streamlining the cleanup or reorganization of animation data.

![ECurveRenamer Window](/EСurvesRemover.png)

## Installation
Copy ECurvesRemover.cs into your Unity project's Editor folder alongside ECurveRenamer.cs.
   
## Usage
Open the editor window through Unity's menu by selecting Escripts > ECurves Remover.

### Curve Deletion
Specify a curve path to delete specific curves from selected animation clips. Include child curves or cut/change the curve's hierarchy as needed.

### Selective Curve Removal
Choose to delete only position, rotation, or scale curves to refine which data is removed from your animations.

### Hierarchy Modification
Easily cut out parts of curve paths or change their hierarchy without deleting them, allowing for flexible reorganization of animation data.

### Asset Database Management
Decide whether to save changes immediately to the Asset Database to ensure all modifications are persisted.

## Quick Removal
1. Select the animation clips you wish to modify in the Project window.
2. Input the curve path you want to delete or modify.
3. Use the toggles to include child curves, cut/change hierarchy, and specify which types of curves (position, rotation, scale) to remove.
4. Click "Delete Curves" to execute the removal or modification.
