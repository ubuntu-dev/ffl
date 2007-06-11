\ ==============================================================================
\
\               nct - the n-Tree cell module in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2007-06-11 05:07:03 $ $Revision: 1.3 $
\
\ ==============================================================================

include ffl/config.fs


[UNDEFINED] nct.version [IF]


include ffl/nnt.fs
include ffl/ncn.fs


( nct = n-Tree cell module )
( The nct module implements a n-Tree that can store cell size data. It extends)
( the base n-Tree module, with cell based words. For adding and removing cells)
( to and from the tree, use the iterator [nci].                               )


1 constant nct.version


( Private words )

: nct-delete-children ( w:dnl - = Delete all nodes [children] in the list )
  BEGIN
    dup dnl-pop dup nil<>
  WHILE
    ncn-free
  REPEAT
  2drop
;


( Tree structure )

nnt% constant nct%  ( - n = Get the required space for the nct data structure )


( Tree creation, initialisation and destruction )

: nct-init     ( w:nct - = Initialise the nct-tree )
  nnt-init
;


: nct-create   ( C: "name" - R: - w:nct = Create a named nct-tree in the dictionary )
  create   here   nct% allot   nct-init
;


: nct-new      ( - w:nct = Create a new nct-tree on the heap )
  nct% allocate  throw  dup nct-init
;


: nct-delete-all  ( w:nct - = Delete all the nodes in the tree )
  dup nnt-root@                    \ walk = nct.root
  BEGIN
    dup nil<>                  \ While walk <> nil Doe
  WHILE
    dup nnn>children           \   If node has children Then
    dnl-first@ dup nil<> IF
      nip                      \     Move walk to first child
    ELSE                       \   Else
      drop
      dup nnn>dnn 
      dnn-next@ dup nil<> IF   \     If node has sibling Then
        nip                    \       Move walk to next sibling
      ELSE                     \     Else
        drop
        dup nnn-parent@ dup nil= IF \  If parent = nil Then (root node)
          swap ncn-free        \         Free root node
        ELSE                   \       Else
          nip dup nnn>children \         Free all children of the parent node
          nct-delete-children
        THEN
      THEN
    THEN
  REPEAT
  drop
  dup nnt>root nil!
      nnt>length 0!
;


: nct-free     ( w:nct - = Free the tree from the heap )
  dup nct-delete-all
  
  free  throw
;


( Member words )

: nct-length@  ( w:nct - u = Get the number of nodes in the tree )
  nnt-length@
;


: nct-empty?   ( w:nct - f = Check for empty tree )
  nnt-empty?
;


( Private words )

: nct+emit-node  ( w:data - = Emit the tree node )
  0 .r [char] ; emit
;


: nct+equal?     ( w:data w:data - w:data f = Check if data is equal to the node data )
  over =
;


: nct+count      ( w:count w:data w:data - w:count w:data = Count if data is equal to the node data )
  over = IF
    swap 1+ swap
  THEN
;


( Tree words )

: nct-execute      ( ... xt w:nct - ... = Execute xt for every node in tree )
  nnt-root@                 \ walk = first
  BEGIN
    dup nil<>               \ while walk<>nil do
  WHILE
    2>r 
    2r@ ncn-cell@
    swap execute            \  execute xt with node
    2r>
    nnn-next                \  walk = walk next
  REPEAT
  2drop
;


: nct-execute?     ( ... xt w:nct - ... f = Execute xt for every node in the tree until xt returns true )
  nnt-root@                 \ walk = first
  false
  BEGIN
    over nil<> over 0= AND  \ while walk<>nil and flag = false do
  WHILE
    drop    
    2>r 
    2r@ ncn-cell@
    swap execute            \  execute xt with node
    2r>
    nnn-next                \  walk = walk next
    rot
  REPEAT
  nip nip
;
  

: nct-count      ( w:data w:nct - n = Count the occurences of the cell data in the tree )
  0 -rot
  ['] nct+count  swap nct-execute drop
;


: nct-has?       ( w:data w:nct - f = Check if the cell data is present in the tree )
  ['] nct+equal? swap nct-execute? nip
;


( Inspection )

: nct-dump     ( w:nct - = Dump the tree )
  dup nnt-dump
  ." nct:" dup . cr
  ."  nodes :" ['] nct+emit-node swap nct-execute cr
;

[THEN]

\ ==============================================================================
 
