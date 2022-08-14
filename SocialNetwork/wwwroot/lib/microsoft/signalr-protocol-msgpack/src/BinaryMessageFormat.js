"use strict";
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
Object.defineProperty(exports, "__esModule", { value: true });
exports.BinaryMessageFormat = void 0;
// Not exported from index.
/** @private */
var BinaryMessageFormat = /** @class */ (function () {
    function BinaryMessageFormat() {
    }
    // The length prefix of binary messages is encoded as VarInt. Read the comment in
    // the BinaryMessageParser.TryParseMessage for details.
    BinaryMessageFormat.write = function (output) {
        var size = output.byteLength || output.length;
        var lenBuffer = [];
        do {
            var sizePart = size & 0x7f;
            size = size >> 7;
            if (size > 0) {
                sizePart |= 0x80;
            }
            lenBuffer.push(sizePart);
        } while (size > 0);
        size = output.byteLength || output.length;
        var buffer = new Uint8Array(lenBuffer.length + size);
        buffer.set(lenBuffer, 0);
        buffer.set(output, lenBuffer.length);
        return buffer.buffer;
    };
    BinaryMessageFormat.parse = function (input) {
        var result = [];
        var uint8Array = new Uint8Array(input);
        var maxLengthPrefixSize = 5;
        var numBitsToShift = [0, 7, 14, 21, 28];
        for (var offset = 0; offset < input.byteLength;) {
            var numBytes = 0;
            var size = 0;
            var byteRead = void 0;
            do {
                byteRead = uint8Array[offset + numBytes];
                size = size | ((byteRead & 0x7f) << (numBitsToShift[numBytes]));
                numBytes++;
            } while (numBytes < Math.min(maxLengthPrefixSize, input.byteLength - offset) && (byteRead & 0x80) !== 0);
            if ((byteRead & 0x80) !== 0 && numBytes < maxLengthPrefixSize) {
                throw new Error("Cannot read message size.");
            }
            if (numBytes === maxLengthPrefixSize && byteRead > 7) {
                throw new Error("Messages bigger than 2GB are not supported.");
            }
            if (uint8Array.byteLength >= (offset + numBytes + size)) {
                // IE does not support .slice() so use subarray
                result.push(uint8Array.slice
                    ? uint8Array.slice(offset + numBytes, offset + numBytes + size)
                    : uint8Array.subarray(offset + numBytes, offset + numBytes + size));
            }
            else {
                throw new Error("Incomplete message.");
            }
            offset = offset + numBytes + size;
        }
        return result;
    };
    return BinaryMessageFormat;
}());
exports.BinaryMessageFormat = BinaryMessageFormat;
