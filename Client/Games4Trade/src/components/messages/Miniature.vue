<template>
    <div class="row">
        <div class="col-3">
            <img :src="`http://localhost:5000/api/users/${message.reciverId}/photo`">
        </div>
        <div class="col-9">
            <div class="row">
                <div class="col-6">
                    <p style="font-weight: bold">
                        {{message.reciver.login}}
                    </p>
                </div>
                <div class="col-6">
                    <small>{{message.dateCreated.substring(2,10)}} </small>
                    <small>{{message.dateCreated.substring(11,16)}}</small>
                </div>
            </div>
            <div class="row">
                <p :class="{newMessage: !message.isDelivered && message.ReciverId === userId}">
                    {{shortenString(message.content)}}</p>
            </div>
        </div>
    </div>
</template>

<script>
export default {
  name: 'Miniature',
  props: ['message', 'userId'],
  methods: {
    shortenString (text) {
      if (text.length > 60) {
        return text.substring(0, 57) + '...'
      }
      return text
    }
  }
}
</script>

<style scoped>
    img {
        width: 4vw;
        height: 4vw;
        object-fit: cover;
    }
    .newMessage {
        font-weight: bold;
    }
</style>
