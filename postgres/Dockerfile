FROM postgres

RUN apt-get update && apt-get install -y \
    git \
    build-essential \
    postgresql-server-dev-all

RUN git clone https://github.com/petere/pguint.git /tmp/pguint && \
    cd /tmp/pguint && \
    make && make install && \
    rm -rf /tmp/pg_ivm
