NBToolkit
Copyright (C) 2011 Justin Aquadro

NBToolkit is a set of offline processing utilities for your MC worlds, unified 
into a single package. The following commands are present:

  help        Get help and usage info for another command
  oregen      Generate structured deposits of a single block type
  replace     Replace one block type with another
  purge       Delete chunks
  dump        Dump parsed chunk data to a readable JSON file
  relight     Recalculate lighting on chunks

All utilities support a set of common options to limit the number of chunks or 
blocks that actually get updated. Such limits include limiting updated chunks 
to a bounding box defined in chunk coordinates, or updating chunks 
conditionally on whether they contain or don't contain some block type.

NBToolkit is built on top of Substrate (https://github.com/jaquadro/Substrate)