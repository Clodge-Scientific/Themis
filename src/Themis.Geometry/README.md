# Themis.Geometry
## Overview
This project encompasses the core geometric and spatial indexing functionality that allows for efficient and reliable representation, analyis, and indexing of common geospatial data types.

The concrete geometric implementations and search trees themselves are intended to provide simple interfaces for common spatial analysis tasks and do not represent a fully composed pipeline.  Instead, ***they are the fundamental building blocks that enable consumers to quickly implement spatial analysis pipelines for datasets of significant scale***.

## Structure
The project can be broken into a number of key components as follows:
- __Themis.Geometry__
    - [Boundary](/Boundary/README.md)
    - [Lines](/Lines/README.md)
    - [Triangles](/Triangles/README.md)
- __Themis.Geometry.Index__
    - [KdTree](/Index/KdTree/README.md)
    - [QuadTree](/Index/QuadTree/README.md)

# Roadmap
## Themis.Geometry
- Improve BoundingBox API - consider splitting into BoundingBox and BoundingCube
- Consider adding builders or factories for each geometry
- Add Delaunay Tesselation that produces a `Triangle` surface (TIN)
## Themis.Geometry.Index
- Improve API of KdTree (build from collection, get only TValue from searches, etc)