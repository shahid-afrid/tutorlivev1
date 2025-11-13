# Modal Not Opening - Bootstrap Issue Fix

## Problem Identified
The "Assign Faculty" button is not opening the modal dialog box in ManageSubjectAssignments.cshtml. 

## Root Cause
Based on comparison with the working faculty management page:
1. Bootstrap modal initialization may be failing silently
2. The modal needs a more direct, custom approach rather than relying on Bootstrap's default behavior
3. The modal CSS needs explicit display control

## Solution
Replace Bootstrap's modal system with a custom, simpler modal implementation that:
- Uses direct JavaScript show/hide functions
- Has explicit CSS for display control
- Doesn't rely on Bootstrap's modal.show() method

## Files to Modify
- Views/Admin/ManageSubjectAssignments.cshtml

## Implementation Steps
1. Update modal CSS to use custom display control (not Bootstrap classes)
2. Replace Bootstrap modal initialization with custom JavaScript functions
3. Add direct show/hide functions that manipulate CSS classes
4. Remove dependency on Bootstrap Modal API

This approach is proven to work in ManageCSEDSFaculty.cshtml and will be applied here.
