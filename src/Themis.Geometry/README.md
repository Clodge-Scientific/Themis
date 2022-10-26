# Themis.Geometry
## Overview
This project encompasses the core geometriic object models and funcitonality that allow for efficient and reliable representation, manipulation, and analysis of common geospatial data types.

## Structure
The project can be broken into a number of key components as follows:

- [Boundary](/src/Themis.Geometry/Boundary/README.md)
- [Lines](/src/Themis.Geometry/Lines/README.md)
- [Triangles](/src/Themis.Geometry/Triangles/README.md)

# Roadmap
## Themis.Geometry
- Improve BoundingBox API - consider splitting into BoundingBox and BoundingCube
- Consider adding builders or factories for each geometry
- Add Delaunay Tesselation that produces a `Triangle` surface (TIN)