Virtual Template System for Episerver
=====================================

Virtual Template system for Episerver allows editors to edit templates in the Episerver UI without the need for a release. 

More information can be found here: https://www.david-tec.com/tag/vts

## Change log

### v3.2.2

- Fix bug where searches are not run if a user presses [enter]

### v3.2.1

- Fix bug where file name searches are not case-insensitive

### v3.2

- Add abilty to search in file contents

### v3.1.1

- Bug fix when deleting content

### v3.1

- Version history showing last 5 versions now visible in the UI
- Icons and colour now used in the UI
- Fixed a bug where an exception would be thrown if the application tried to access a file in Application_Start()
- Changed URL segment to VTS
- Add sub-resource integrity hash for the ace-diff library

### v3.0.3

- UI bug fix for black text when should be white
- Issue when tempalte paths contained ~ signs

### v3.0

- Upgraded to Bootstrap 4
- Templates are now saved in the Episerver content repository (no longer the DDS)
- Added "changed by" column in list view
- Added "Save and close" button
- Save button now stays on current view
- Moved and renamed some button layouts
- Added a close X button to the top right
- Fixed a file encoding bug when reading certain file encoding types from storage
- Fixed an issue when comparing files that had embedded </script> tags
- Number of refactoring tasks

### v2.1

- Added the ability to compare and merge the edited template with the one on disk
- Minor code tidy up/refactoring

### v2.0

- Compatible with Episerver 11
- Abiltiy to search templates (thanks to Validis for some inspiration from DBLocalisationProvider)
- Code highlighting when viewing and editing thanks to Ace Editor
- Moved the templates link under the CMS menu
- Some wording changed to be more editor friendly
