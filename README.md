# Themis

- [Themis](#themis)
    - [Overview](#overview)
- [Components](#components)
    - [Themis.Geometry](#themisgeometry)
    - [Themis.Index](#themisindex)

## Overview
*Themis* is intended to help enforce a minutia of order upon the chaos of geospatial software development by delivering hardened & efficient components that can be reliably connected to perform variable workloads.

The concrete geometric implementations and search trees themselves are intended to provide simple interfaces for common spatial analysis tasks and do not represent a fully composed pipeline.  Instead, they are the fundamental building blocks that enable consumers to quickly implement spatial analysis pipelines for datasets of significant scale.

# Components

## Themis.Geometry

**Themis.Geometry** is responsible for the core geometric object representations such as `Triangles` or `LineSegments` that are used and abused throughout the Themis ecosystem.

See [Themis.Geometry](src/Themis.Geometry)

## Themis.Index

**Themis.Index** is responsible for exposing spatial indexing APIs such as `KdTrees` and `QuadTrees` that can be used to index and query various spatial datasets with maximum efficiency.

See [Themis.Index](src/Themis.Index/)