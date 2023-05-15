git pull
docker build -t wdora/mocksocket-server .
docker rm -f my-mock-server
docker run -d --restart always --network host --name=my-mock-server wdora/mocksocket-server