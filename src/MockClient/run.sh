docker rm -f mockclient
docker run --network host -d --name mockclient wdora/mockclient:0.1