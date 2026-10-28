# RotationSolverRebornButBetter

An unofficial fork of [Rotation Solver Reborn](https://github.com/FFXIV-CombatReborn/RotationSolverReborn)
by The Combat Reborn Team. All credit for the plugin goes to the original authors. This repo is not affiliated with
or supported by them; please don't report issues with this fork upstream.

## Why this fork exists

The official plugin only runs the rotations built into it. This fork is an automated build
with a small addition that loads extra rotation DLLs from
`%APPDATA%\XIVLauncher\pluginConfigs\RotationSolver\CustomRotations\`, so custom rotations can be
maintained separately instead of in a full fork of the plugin. Everything else is unchanged.

## How it works

`.github/workflows/release.yml` runs every 4 hours (and on changes to the fork files):

1. Finds the newest upstream release tag (e.g. `7.5.6.13`).
2. Clones that tag and runs `apply-fork.ps1`, which adds `fork-src/CustomRotationLoader.cs`, hooks it into
   `RotationUpdater` and applies the branding via exact text replacements.
3. Builds as version `<major>.<minor>.<build>.<revision*100 + PATCH_REVISION>` (e.g. `7.5.6.1301`).
4. Publishes a GitHub release and updates `repo.json`.

If a line `apply-fork.ps1` replaces disappears upstream, the run fails with the missing text and GitHub notifies you.
Bump `PATCH_REVISION` when the fork changes without a new upstream release, so Dalamud sees an update.

## Install

1. Uninstall the official Rotation Solver Reborn (same internal name; settings are kept).
2. Dalamud Settings > Experimental > Custom Plugin Repositories, add
   `https://raw.githubusercontent.com/IndividualGather/RotationSolverRebornButBetter/main/repo.json`.
3. Install "RotationSolverRebornButBetter".
