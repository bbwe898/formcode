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

local flags = {}
local kHttpHeaderSent = 1
local kHttpHeaderRecived = 2
local password = settings.password
local username = settings.username

function wa_lua_on_flags_cb(ctx)
    return DIRECT_WRITE
end

function wa_lua_on_handshake_cb(ctx)
    local uuid = ctx_uuid(ctx)

    if flags[uuid] == kHttpHeaderRecived then
        return true
    end

    if flags[uuid] ~= kHttpHeaderSent then
        local host = ctx_address_host(ctx)
        local port = ctx_address_port(ctx)
        local res = 'CONNECT ' .. host .. ':' .. port .. ' HTTP/1.1\r\n' ..
                    'Host: boot-video2.xuexi.cn\r\n' ..
                    'Proxy-Connection: Keep-Alive\r\n'..
                    'User-Agent: Channel/201200 language/zh-Hans-CN Device/XueXi XueXi/2.43.0 CPUArch/arm64e(64bit) osInfo/iOS(13.0) BundleID/cn.xuexi.qg BuildID/26885517\r\n\r\n'
        ctx_write(ctx, res)
        flags[uuid] = kHttpHeaderSent
    end

    return false
end

function wa_lua_on_read_cb(ctx, buf)
    ctx_debug('wa_lua_on_read_cb')
    local uuid = ctx_uuid(ctx)
    if flags[uuid] == kHttpHeaderSent then
        flags[uuid] = kHttpHeaderRecived
        return HANDSHAKE, nil
    end
    return DIRECT, buf
end

function wa_lua_on_write_cb(ctx, buf)
	local host = ctx_address_host(ctx)
	local port = ctx_address_port(ctx)

	if ( is_http_request(buf) == 1 ) then
		local s, e = find(buf, '\r\n')
		local line1 = sub(buf, 0, e)
		local line1tail = sub(buf, e + 1)
		local s1, e1 = find(line1tail, '\r\n')
		local line2 = sub(line1tail, 0, e1)
		local line2tail = sub(line1tail, e + 1)
		local s2, e2 = find(line2tail, '\r\n')
		local line3 = sub(line2tail, 0, e1)
		local line3tail = sub(line2tail, e + 1)
		local s, e = find(beginagent, '\r\n')

		buf = line1 .. line2 ..
			'User-Agent: ' .. username .. '\r\n' ..
			line3tail
	end
	return DIRECT, buf
end

function wa_lua_on_close_cb(ctx)
    ctx_debug('wa_lua_on_close_cb')
    local uuid = ctx_uuid(ctx)
    flags[uuid] = nil
    ctx_free(ctx)
    return SUCCESS
end
