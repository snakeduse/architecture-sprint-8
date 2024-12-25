-module(reports_utils).

-export([
    get_pub_key/2
]).

get_pub_key(KeycloakServer, RealmName) ->
    Url = io_lib:format("~s/realms/~s", [KeycloakServer, RealmName]),
    {ok, {{"HTTP/1.1", 200, "OK"}, _Headers, HttpBodyResult}} = httpc:request(Url),
    #{ <<"public_key">> := PK } = jsone:decode(unicode:characters_to_binary(HttpBodyResult)),
    PK.
