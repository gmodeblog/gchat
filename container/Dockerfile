FROM centos:centos7
MAINTAINER mito <makoto.ito@gmodecorp.com>

USER root

# packages
RUN yum -y update && yum -y upgrade && yum install -y epel-release yum-utils && yum clean all
RUN yum install -y git tar gcc make openssl openssl-devel readline libffi libffi-devel \
    sudo openssh-server net-tools python-setuptools nginx telnet tree && \
    yum clean all

# mono packages
RUN rpm --import "http://keyserver.ubuntu.com/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF" && \
    yum-config-manager --add-repo http://download.mono-project.com/repo/centos/ && \
    yum install -y mono-complete && yum clean all

# locale and timezone.
RUN cp /usr/share/zoneinfo/Asia/Tokyo /etc/localtime && localedef -f UTF-8 -i ja_JP ja_JP.utf8

# sshd
RUN sed -ri 's/UsePAM yes/#UsePAM yes/g' /etc/ssh/sshd_config && \
    ssh-keygen -q -N "" -t rsa -f /etc/ssh/ssh_host_rsa_key && \
    ssh-keygen -q -N "" -t ecdsa -f /etc/ssh/ssh_host_ecdsa_key

# supervisor
RUN easy_install pip && pip install supervisor && \
    mkdir -p /etc/supervisor/conf.d && chmod 777 /etc/supervisor/conf.d
ADD supervisord.conf /etc/supervisord.conf

# operation user
ENV APPUSER appuser
RUN useradd $APPUSER
RUN echo "$APPUSER:$APPUSER" | chpasswd && \
    echo "$APPUSER ALL=(ALL) NOPASSWD:ALL" >> /etc/sudoers

USER appuser
RUN echo "LANG=ja_jp.UTF-8" >>  /home/appuser/.bash_profile

ENV RBENV_VERSION 2.2.1
RUN cd /home/appuser && \
    git clone https://github.com/sstephenson/rbenv.git .rbenv && \
    git clone https://github.com/sstephenson/ruby-build.git /home/appuser/.rbenv/plugins/ruby-build && \
    echo 'export PATH="/home/appuser/.rbenv/bin:$PATH"' >> /home/appuser/.bash_profile && \
    echo 'eval "$(rbenv init -)"' >> /home/appuser/.bash_profile

RUN source /home/appuser/.bash_profile && cd /home/appuser && \
    rbenv install $RBENV_VERSION && \
    rbenv rehash && \
    rbenv global $RBENV_VERSION && \
    gem install highline bundler thor && rbenv rehash

# chatapp
VOLUME ["/code"]

USER root
RUN ln -s /code/container/supervisor.conf /etc/supervisor/conf.d/chatapp.conf

EXPOSE 22 80 57200
CMD /usr/bin/supervisord -c /etc/supervisord.conf
