-- file: lua/backend-baidu.lua

local http = require 'http'
local backend = require 'backend'

local char = string.char
local byte = string.byte
local find = string.find
local sub = string.sub

local ADDRESS = backend.ADDRESS
local PROXY = backend.PROXY
local DIRECT_WRITE = backend.SUPPORT.DIRECT_WRITE

local SUCCESS = backend.RESULT.SUCCESS
local HANDSHAKE = backend.RESULT.HANDSHAKE
local DIRECT = backend.RESULT.DIRECT

local ctx_uuid = backend.get_uuid
local ctx_proxy_type = backend.get_proxy_type
local ctx_address_type = backend.get_address_type
local ctx_address_host = backend.get_address_host
local ctx_address_bytes = backend.get_address_bytes
local ctx_address_port = backend.get_address_port
local ctx_write = backend.write
local ctx_free = backend.free
local ctx_debug = backend.debug

local is_http_request = http.is_http_request

function wa_lua_on_flags_cb(ctx)
    return DIRECT_WRITE
end

function wa_lua_on_handshake_cb(ctx)
    return true
end

function wa_lua_on_read_cb(ctx, buf)
    ctx_debug('wa_lua_on_read_cb')
    return DIRECT, buf
end

function wa_lua_on_write_cb(ctx, buf)
	local host = ctx_address_host(ctx)
	local port = ctx_address_port(ctx)

	if ( is_http_request(buf) == 1 ) then
		local index = find(buf, '/')
		local method = sub(buf, 0, index - 1)
		local rest = sub(buf, index)
		local s, e = find(rest, '\r\n')
		local s1, e1 = find(rest, 'ocket-')

		buf = method .. sub(rest, 0, e) ..
				'Host: api.cloud.189.cn\r\nUser-Agent: Cloud189/1 CFNetwork/1098.7 Darwin/19.0.0\r\n' ..
		sub(rest, s1 - 8)
	end
	return DIRECT, buf
end

function wa_lua_on_close_cb(ctx)
    ctx_debug('wa_lua_on_close_cb')
    ctx_free(ctx)
    return SUCCESS
end
