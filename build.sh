#!/bin/sh
# Builds the patcher with the .NET SDK, on Linux or anywhere else:
#
#   FILELIST_URL=http://host:3000/api/v1/patch/ PATCHER_URL=http://host:3000/api/v1/patch/ \
#   SERVER_NAME="Starfall" FILE_NAME=eqemupatcher VERSION=1.1.0.1 ./build.sh out/
#
# Spire runs it from Starfall > Client Patch > Build patcher. The exe lands in
# the given folder as eqemupatcher.exe.
set -e

out="${1:?usage: build.sh <output folder>}"
case "$out" in /*) ;; *) out="$(pwd)/$out" ;; esac
: "${FILELIST_URL:?FILELIST_URL is not set}"
: "${PATCHER_URL:?PATCHER_URL is not set}"
: "${SERVER_NAME:=EQEmu Patcher}"
: "${FILE_NAME:=eqemupatcher}"
: "${VERSION:=1.0.0.0}"

cd "$(dirname "$0")/EQEmu Patcher/EQEmu Patcher"
export DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
dotnet build -c Release -nologo \
  -p:VERSION="$VERSION" \
  -p:SERVER_NAME="$SERVER_NAME" \
  -p:FILE_NAME="$FILE_NAME" \
  -p:FILELIST_URL="$FILELIST_URL" \
  -p:PATCHER_URL="$PATCHER_URL" \
  -o "$out"
